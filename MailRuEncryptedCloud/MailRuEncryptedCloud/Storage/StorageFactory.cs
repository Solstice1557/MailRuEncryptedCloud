namespace MailRuEncryptedCloud.Storage
{
    public class StorageFactory : IStorageFactory
    {
        public IStorage CreateLocalStorage(string basePath)
        {
            return new FileSystemStorage(basePath);
        }
    }
}
