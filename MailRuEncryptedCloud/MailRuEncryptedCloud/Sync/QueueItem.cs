namespace MailRuEncryptedCloud.Sync
{
    using MailRuEncryptedCloud.DB.Models;
    using MailRuEncryptedCloud.Storage;

    public class QueueItem
    {
        public IStorage LocalStorage { get; set; }

        public string LocalPath { get; set; }

        public IStorage RemoteStorage { get; set; }

        public string RemotePath { get; set; }

        public QueueItemStatus Status { get; set; }

        public EncryptedFile EncryptedFile { get; set; }

        public string Message { get; set; }
    }
}
