namespace MailRuEncryptedCloud.DB.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Device
    {
        [Key]
        public int Id { get; set; }

        public string DeviceName { get; set; }

        public virtual ICollection<DeviceRootFolder> DeviceRootFolders { get; set; }
    }
}
