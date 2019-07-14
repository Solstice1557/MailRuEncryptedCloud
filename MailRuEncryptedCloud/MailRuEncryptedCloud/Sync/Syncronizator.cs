namespace MailRuEncryptedCloud.Sync
{
    using System.Threading;
    using System.Threading.Tasks;

    public class Syncronizator
    {
        private readonly ISyncFoldersReader syncFoldersReader;

        private readonly ISyncQueueProcessor syncQueueProcessor;

        public Syncronizator(
            ISyncFoldersReader syncFoldersReader, 
            ISyncQueueProcessor syncQueueProcessor)
        {
            this.syncFoldersReader = syncFoldersReader;
            this.syncQueueProcessor = syncQueueProcessor;
        }

        public Task FullBackup()
        {
            return FullBackup(CancellationToken.None);
        }

        public async Task FullBackup(CancellationToken cancellationToken)
        {
            var queue = new SyncConcurrentQueue();
            var readFilesTask = this.syncFoldersReader.Sync(queue, cancellationToken);
            var uploadItemsTask = this.syncQueueProcessor.ProcessQueue(queue, cancellationToken);

            await readFilesTask;
            this.syncQueueProcessor.ReaderFinished();
            await uploadItemsTask;
        }
    }
}
