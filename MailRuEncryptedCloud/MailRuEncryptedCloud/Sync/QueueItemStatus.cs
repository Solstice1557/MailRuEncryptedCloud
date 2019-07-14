namespace MailRuEncryptedCloud.Sync
{
    public enum QueueItemStatus
    {
        WaitingForUpload,
        WaitingForDownload,
        Uploading,
        Downloading,
        RetryUploading,
        RetryDownloading,
        Finished,
        Error
    }
}
