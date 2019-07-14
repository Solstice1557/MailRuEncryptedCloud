namespace MailRuEncryptedCloud.Sync
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISyncFoldersReader
    {
        Task Sync(SyncConcurrentQueue queue, CancellationToken cancellationToken);
    }
}
