namespace MailRuEncryptedCloud.Storage
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MailRuCloudClient;

    public class MailRuCloudStorage : IStorage
    {
        private readonly CloudClient cloudClient;

        public MailRuCloudStorage(CloudClient cloudClient)
        {
            this.cloudClient = cloudClient;
        }

        public Task<List<StorageItem>> GetFolderChilds(string folderPath)
        {
            throw new System.NotImplementedException();
        }

        public Task CreateFolder(StorageFolder parent, string folderName)
        {
            throw new System.NotImplementedException();
        }

        public Task UploadFile(StorageFolder parent, string localPath, string storageFileName)
        {
            throw new System.NotImplementedException();
        }

        public Task DownloadFile(string localPath, StorageFile storageFile)
        {
            throw new System.NotImplementedException();
        }
    }
}
