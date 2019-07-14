namespace MailRuEncryptedCloud.DB.Models
{
    using System.ComponentModel.DataAnnotations;

    public class DeviceRootFolder
    {
        [Key]
        public int DeviceId { get; set; }

        [Key]
        public int FolderId { get; set; }

        [StringLength(2000)]
        public string RootPath { get; set; }

        public virtual Device Device { get; set; }

        public virtual EncryptedFolder Folder { get; set; }
    }
}
