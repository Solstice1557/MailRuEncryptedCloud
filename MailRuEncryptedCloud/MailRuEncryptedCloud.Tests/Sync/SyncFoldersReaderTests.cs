namespace MailRuEncryptedCloud.Tests.Sync
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using MailRuEncryptedCloud.DB.Models;
    using MailRuEncryptedCloud.Storage;
    using MailRuEncryptedCloud.Sync;

    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class SyncFoldersReaderTests
    {
        private const string BasePath = @"c:\some\path";

        private const string DeviceName = "SomeDevice";

        private const string RootFolder = "RootFolder";

        private TestDbContext dbContext;

        private Mock<IStorage> remoteStorageMock;

        private Mock<IStorage> localStorageMock;

        private Mock<IStorageFactory> storageFactoryMock;

        private EncryptedFolder rootFolder;

        [TestCase]
        public async Task OneRemoteFileSync()
        {
            var encryptedFile = new EncryptedFile
                                    {
                                        Name = "someFile.txt",
                                        EncryptedName = "encryptFileName",
                                        Created = DateTime.UtcNow,
                                        Folder = this.rootFolder
                                    };
            this.dbContext.Files.Add(encryptedFile);

            this.localStorageMock.Setup(x => x.GetFolderChilds(RootFolder))
                .Returns(Task.FromResult(new List<StorageItem>()));
            
            await this.dbContext.SaveChangesAsync();
            var remoteStorageFile = new StorageFile
                                        {
                                            Name = encryptedFile.EncryptedName,
                                            Modified = DateTime.UtcNow,
                                            FullPath = $"{this.rootFolder.EncryptedName}/{encryptedFile.EncryptedName}",
                                            Size = 10
                                        };

            this.remoteStorageMock.Setup(x => x.GetFolderChilds(this.rootFolder.EncryptedName))
                .Returns(Task.FromResult(new List<StorageItem> { remoteStorageFile }));

            var reader = new SyncFoldersReader(
                this.dbContext,
                this.remoteStorageMock.Object,
                DeviceName,
                this.storageFactoryMock.Object);

            var queue = new SyncConcurrentQueue();

            await reader.Sync(queue, CancellationToken.None);
            Assert.IsFalse(queue.IsEmpty);
            Assert.AreEqual(1, queue.Count);
            var queueItem = queue.TryDequeue();
            Assert.IsNotNull(queueItem);
            Assert.IsNotNull(queueItem.EncryptedFile);
            Assert.AreEqual(encryptedFile.Id, queueItem.EncryptedFile.Id);
            Assert.AreEqual($@"{RootFolder}\{encryptedFile.Name}", queueItem.LocalPath); 
            Assert.AreEqual($"{this.rootFolder.EncryptedName}/{encryptedFile.EncryptedName}", queueItem.RemotePath);
            Assert.AreEqual(QueueItemStatus.WaitingForDownload, queueItem.Status);
        }

        [SetUp]
        public async Task SetUp()
        {
            this.remoteStorageMock = new Mock<IStorage>();
            this.localStorageMock = new Mock<IStorage>();

            this.storageFactoryMock = new Mock<IStorageFactory>();
            this.storageFactoryMock.Setup(x => x.CreateLocalStorage(BasePath)).Returns(this.localStorageMock.Object);

            this.dbContext = new TestDbContext();
            var deviceRootFolder = new DeviceRootFolder
                                       {
                                           RootPath = BasePath,
                                           Folder = new EncryptedFolder
                                                        {
                                                            Name = RootFolder,
                                                            EncryptedName = Guid.NewGuid().ToString(),
                                                            Created = DateTime.UtcNow,
                                                            Modified = DateTime.UtcNow,
                                                        }
                                       };
            this.dbContext.Devices.Add(
                new Device
                    {
                        DeviceName = DeviceName,
                        DeviceRootFolders = new List<DeviceRootFolder>
                                                {
                                                    deviceRootFolder
                                                }
                    });
            await this.dbContext.SaveChangesAsync();

            this.rootFolder = await this.dbContext.DeviceRootFolders
                .Where(dr => dr.Device.DeviceName == DeviceName)
                .Select(dr => dr.Folder)
                .FirstOrDefaultAsync(f => f.Name == RootFolder);
        }

        [TearDown]
        public void TearDown()
        {
            this.dbContext?.Dispose();
            this.dbContext = null;
            this.remoteStorageMock = null;
            this.localStorageMock = null;
            this.storageFactoryMock = null;
            this.rootFolder = null;
        }
    }
}
