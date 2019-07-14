namespace MailRuEncryptedCloud.Storage
{
    public interface IStorageFactory
    {
        IStorage CreateLocalStorage(string basePath);
    }
}