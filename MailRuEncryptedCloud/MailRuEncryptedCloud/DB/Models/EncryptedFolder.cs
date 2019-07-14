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

        public int? RootId { get; set; }

        [StringLength(50)]
        public string EncryptedName { get; set; }

        [StringLength(260)]
        public string Name { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public virtual EncryptedFolder Parent { get; set; }

        public virtual EncryptedFolder Root { get; set; }

        public virtual ICollection<EncryptedFolder> Folders { get; set; }

        public virtual ICollection<EncryptedFolder> AllFolders { get; set; }

        public virtual ICollection<EncryptedFile> Files { get; set; }

        public virtual ICollection<DeviceRootFolder> DeviceRootFolders { get; set; }
    }
}
