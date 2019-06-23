namespace MailRuEncryptedCloud.DB.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class EncryptedFile
    {
        [Key]
        public int Id { get; set; }

        public int FolderId { get; set; }

        [StringLength(50)]
        public string EncryptedName { get; set; }

        [StringLength(260)]
        public string Name { get; set; }

        [MaxLength(256)]
        public byte[] EnryptedHash { get; set; }

        [MaxLength(256)]
        public byte[] Hash { get; set; }

        public DateTime Modified { get; set; }

        public DateTime Created { get; set; }

        public EncryptedFolder Folder { get; set; }
    }
}
