namespace MailRuEncryptedCloud.DB.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EncryptedFolder
    {
        [Key]
        public int Id { get; set; }

        public int? ParentId { get; set; }

        [StringLength(50)]
        public string EncryptedName { get; set; }

        [StringLength(260)]
        public string Name { get; set; }

        public DateTime Created { get; set; }

        public EncryptedFolder Parent { get; set; }

        public ICollection<EncryptedFolder> Folders { get; set; }

        public ICollection<EncryptedFile> Files { get; set; }
    }
}
