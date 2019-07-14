namespace MailRuEncryptedCloud.Sync
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISyncQueueProcessor
    {
        event EventHandler<QueueItem> ItemProcessed;

        Task ProcessQueue(SyncConcurrentQueue queue, CancellationToken cancellationToken);

        void ReaderFinished();
    }
}
