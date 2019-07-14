namespace MailRuEncryptedCloud.Sync
{
    using System;
    using System.Collections.Concurrent;

    public class SyncConcurrentQueue
    {
        private readonly ConcurrentQueue<QueueItem> queue = new ConcurrentQueue<QueueItem>();

        public event EventHandler Changed;

        public bool IsEmpty => this.queue.IsEmpty;

        public int Count => this.queue.Count;

        public QueueItem TryDequeue()
        {
            var result = this.queue.TryDequeue(out var item);
            if (!result)
            {
                return null;
            }

            this.OnChanged();
            return item;
        }

        public void Enqueue(QueueItem item)
        {
            this.queue.Enqueue(item);
            this.OnChanged();
        }

        private void OnChanged()
        {
            this.Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
