namespace MailRuEncryptedCloud.Storage
{
    using System;

    public class StorageFile : StorageItem
    {
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets last modified time of file in UTC format.
        /// </summary>
        public DateTime Modified { get; set; }

        public long Size { get; set; }
    }
}
