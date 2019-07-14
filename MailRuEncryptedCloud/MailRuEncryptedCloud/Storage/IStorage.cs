namespace MailRuEncryptedCloud.Storage
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IStorage
    {
        Task<List<StorageItem>> GetFolderChilds(string folderPath);

        Task CreateFolder(StorageFolder parent, string folderName);

        Task UploadFile(StorageFolder parent,  string localPath, string storageFileName);

        Task DownloadFile(string localPath, StorageFile storageFile);
    }
}
