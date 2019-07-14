namespace MailRuEncryptedCloud.Storage
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class FileSystemStorage : IStorage
    {
        private readonly string basePath;

        public FileSystemStorage(string basePath)
        {
            this.basePath = basePath;
        }

        public Task<List<StorageItem>> GetFolderChilds(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return Task.FromResult(new List<StorageItem>());
            }

            folderPath = Path.Combine(this.basePath, folderPath);
            if (!Directory.Exists(folderPath))
            {
                return Task.FromResult(new List<StorageItem>());
            }

            var d = new DirectoryInfo(folderPath);
            var files = d.GetFiles()
                .Select(fi => 
                    new StorageFile
                      {
                          Name = fi.Name,
                          Modified = fi.LastWriteTimeUtc,
                          Size = fi.Length,
                          FullPath = fi.FullName
                      })
                .Cast<StorageItem>();
            var directories = d.GetDirectories()
                .Select(di =>
                    new StorageFolder
                        {
                            Name = di.Name,
                            FullPath = di.FullName
                        })
                .Cast<StorageItem>();

            return Task.FromResult(files.Union(directories).ToList());
        }

        public Task CreateFolder(StorageFolder parent, string folderName)
        {
            var path = Path.Combine(this.basePath, parent.FullPath, folderName);
            Directory.CreateDirectory(path);
            return Task.CompletedTask;
        }

        public Task UploadFile(StorageFolder parent, string localPath, string storageFileName)
        {
            var path = Path.Combine(this.basePath, parent.FullPath, storageFileName);
            File.Copy(localPath, path);
            return Task.CompletedTask;
        }

        public Task DownloadFile(string localPath, StorageFile storageFile)
        {
            var path = Path.Combine(this.basePath, storageFile.FullPath);
            File.Copy(localPath, path);
            return Task.CompletedTask;
        }
    }
}
