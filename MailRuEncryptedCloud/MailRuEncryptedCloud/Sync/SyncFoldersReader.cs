namespace MailRuEncryptedCloud.Sync
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using MailRuEncryptedCloud.DB;
    using MailRuEncryptedCloud.DB.Extensions;
    using MailRuEncryptedCloud.DB.Models;
    using MailRuEncryptedCloud.Storage;

    using Microsoft.EntityFrameworkCore;

    public class SyncFoldersReader : ISyncFoldersReader
    {
        private readonly DirDbContext db;

        private readonly IStorage remoteStorage;

        private readonly IStorageFactory storageFactory;

        private readonly string deviceName;

        public SyncFoldersReader(
            DirDbContext db,
            IStorage remoteStorage,
            string deviceName,
            IStorageFactory storageFactory)
        {
            this.db = db;
            this.remoteStorage = remoteStorage;
            this.deviceName = deviceName;
            this.storageFactory = storageFactory;
        }

        public async Task Sync(SyncConcurrentQueue queue, CancellationToken cancellationToken)
        {
            var device = await this.db.Devices
                .Include(d => d.DeviceRootFolders)
                    .ThenInclude(d => d.Folder)
                .Where(d => d.DeviceName == this.deviceName)
                .FirstOrDefaultAsync(cancellationToken);
            if (device == null)
            {
                this.db.Devices.Add(new Device { DeviceName = this.deviceName });
                await this.db.SaveChangesAsync(cancellationToken);
                return;
            }

            if (device.DeviceRootFolders.Count == 0)
            {
                return;
            }

            foreach (var deviceDeviceRootFolder in device.DeviceRootFolders)
            {
                await this.SyncFolder(
                    queue,
                    cancellationToken,
                    deviceDeviceRootFolder.Folder,
                    this.storageFactory.CreateLocalStorage(deviceDeviceRootFolder.RootPath));
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        private async Task SyncFolder(
            SyncConcurrentQueue queue, 
            CancellationToken cancellationToken,
            EncryptedFolder folder,
            IStorage localStorage)
        {
            var remoteFolderPath = folder.GetRelativeEncryptedPath();
            var localFolderPath = folder.GetRelativePath();
            var remoteItems = await this.remoteStorage.GetFolderChilds(remoteFolderPath);
            var localItems = await localStorage.GetFolderChilds(localFolderPath);
            foreach (var remoteItem in remoteItems)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                switch (remoteItem)
                {
                    case StorageFile remoteFile:
                        this.ProcessRemoteFile(
                            remoteFile,
                            localItems,
                            folder,
                            queue,
                            localStorage,
                            localFolderPath);
                        break;
                    case StorageFolder remoteFolder:
                        var encryptedFolder = folder.Folders.FirstOrDefault(f => f.EncryptedName == remoteFolder.Name);
                        if (encryptedFolder == null)
                        {
                            // todo add sync error
                            continue;
                        }

                        await this.SyncFolder(queue, cancellationToken, encryptedFolder, localStorage);
                        break;
                    default:
                        throw new NotImplementedException($"Failed to process emelent of type {remoteItem.GetType()}");
                }
            }

            foreach (var localItem in localItems)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                switch (localItem)
                {
                    case StorageFile localFile:
                        this.ProcessLocalFile(
                            localFile,
                            remoteItems,
                            folder,
                            queue,
                            localStorage,
                            remoteFolderPath);
                        break;
                    case StorageFolder localFolder:
                        var encryptedFolder = folder.Folders.FirstOrDefault(f => f.Name == localFolder.Name);
                        if (encryptedFolder == null)
                        {
                            // new local folder
                            encryptedFolder = new EncryptedFolder
                                                  {
                                                      Name = localFolder.Name,
                                                      EncryptedName = Guid.NewGuid().ToString(),
                                                      RootId = folder.RootId ?? folder.Id,
                                                      Root = folder.Root ?? folder,
                                                      ParentId = folder.Id,
                                                      Parent = folder,
                                                      Created = DateTime.UtcNow,
                                                      Modified = DateTime.UtcNow
                                                  };
                            encryptedFolder = this.db.Add(encryptedFolder).Entity;
                            await this.db.SaveChangesAsync(cancellationToken);
                        }
                        else
                        {
                            var remoteFolderExists = remoteItems.OfType<StorageFolder>()
                                .Any(f => f.Name == encryptedFolder.EncryptedName);
                            if (remoteFolderExists)
                            {
                                // skip existing remote folders
                                continue;
                            }
                        }

                        await SyncFolder(queue, cancellationToken, encryptedFolder, localStorage);
                        break;
                    default:
                        throw new NotImplementedException($"Failed to process emelent of type {localItem.GetType()}");
                }
            }
        }

        private void ProcessLocalFile(
            StorageFile localFile,
            List<StorageItem> remoteItems,
            EncryptedFolder folder,
            SyncConcurrentQueue queue,
            IStorage localStorage,
            string remoteFolderPath)
        {
            var encryptedFile = folder.Files.FirstOrDefault(f => f.Name == localFile.Name);
            if (encryptedFile == null)
            {
                // file doesn't exists in remote
                encryptedFile = new EncryptedFile
                                    {
                                        Name = localFile.Name,
                                        Created = localFile.Modified,
                                        Modified = localFile.Modified,
                                        EncryptedName = Guid.NewGuid().ToString(),
                                        UnencryptedSize = localFile.Size,
                                        // todo Hash = 
                                        FolderId = folder.Id,
                                        Folder = folder
                                    };
                encryptedFile = this.db.Files.Add(encryptedFile).Entity;
                this.db.SaveChangesAsync();
            }
            else
            {
                var remoteExists = remoteItems.Any(x => x.Name == encryptedFile.EncryptedName);
                if (remoteExists)
                {
                    // already processed at remote processing
                    return;
                }
            }

            queue.Enqueue(
                new QueueItem
                    {
                        Status = QueueItemStatus.WaitingForUpload,
                        LocalStorage = localStorage,
                        LocalPath = localFile.FullPath,
                        RemotePath = Path.Combine(remoteFolderPath, encryptedFile.EncryptedName),
                        RemoteStorage = this.remoteStorage,
                        EncryptedFile = encryptedFile
                    });
        }

        private void ProcessRemoteFile(
            StorageFile remoteFile,
            List<StorageItem> localItems,
            EncryptedFolder folder,
            SyncConcurrentQueue queue,
            IStorage localStorage,
            string localFolderPath)
        {
            var encryptedFile = folder.Files.FirstOrDefault(f => f.EncryptedName == remoteFile.Name);
            if (encryptedFile == null)
            {
                // remote file does not exists in db, we can't recrypt it
                // todo add sync error
                return;
            }

            var localPath = Path.Combine(localFolderPath, encryptedFile.Name);
            var localItem = localItems.OfType<StorageFile>().FirstOrDefault(l => l.Name == encryptedFile.Name);
            var itemStatus = GetItemStatus(localItem, remoteFile, encryptedFile);
            if (itemStatus != null)
            {
                queue.Enqueue(
                    new QueueItem
                        {
                            Status = itemStatus.Value,
                            LocalStorage = localStorage,
                            LocalPath = localPath,
                            RemotePath = remoteFile.FullPath,
                            RemoteStorage = this.remoteStorage,
                            EncryptedFile = encryptedFile
                        });
            }
        }

        private static QueueItemStatus? GetItemStatus(
            StorageFile localFile, 
            StorageFile remoteFile,
            EncryptedFile encryptedFile)
        {
            if (localFile == null)
            {
                // file does not existis on local storage, need to download
                return QueueItemStatus.WaitingForDownload;
            }

            if (remoteFile == null)
            {
                // file does not existis on remote storage, need to upload
                return QueueItemStatus.WaitingForUpload;
            }

            if (localFile.Modified > encryptedFile.Modified)
            {
                // local file is newer than in db
                return QueueItemStatus.WaitingForUpload;
            }

            if (localFile.Modified < encryptedFile.Modified)
            {
                // remote file is newer than in db
                return QueueItemStatus.WaitingForDownload;
            }

            // todo check hashes
            return null;
        }
    }
}
