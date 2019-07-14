namespace MailRuEncryptedCloud.Sync
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using MailRuCloudClient;

    using MailRuEncryptedCloud.DB;
    using MailRuEncryptedCloud.DB.Models;
    using MailRuEncryptedCloud.Encryption;

    using Microsoft.EntityFrameworkCore;

    public class Backuper
    {
        private readonly DirDbContext db;

        private readonly IEncryptor encryptor;

        private readonly CloudClient cloudClient;

        public Backuper(
            DirDbContext db, 
            IEncryptor encryptor,
            CloudClient cloudClient)
        {
            this.db = db;
            this.encryptor = encryptor;
            this.cloudClient = cloudClient;
        }

        public async Task BackupFolder(string folderPath)
        {
            /*if (string.IsNullOrEmpty(folderPath))
            {
                throw new ArgumentNullException(nameof(folderPath));
            }

            if (!Directory.Exists(folderPath))
            {
                throw new ArgumentException("Folder does not exists", nameof(folderPath));
            }

            await this.db.Database.MigrateAsync();

            var rootFolder = await this.db.Folders
                .Include(f => f.Files)
                .Where(f => f.ParentId == null 
                            && f.Root == null
                            && f.RootPath == folderPath)
                .SingleOrDefaultAsync();
            List<EncryptedFolder> allFolders = null;
            if (rootFolder == null)
            {
                rootFolder = new EncryptedFolder
                                 {
                                     Name = Path.GetFileName(folderPath),
                                     EncryptedName = this.encryptor.GenerateRandomString(),
                                     RootId = null,
                                     ParentId = null,
                                     Created = Directory.GetCreationTime(folderPath),
                                     RootPath = folderPath
                                 };
                this.db.Folders.Add(rootFolder);
                await this.db.SaveChangesAsync();
            }
            else
            {

            }

            //var allFolderUnderRoot = 

            */
        }

        /*private async Task SyncFiles(EncryptedFolder folder, )
        {
            this.cloudClient.GetFolder()
        }*/
    }
}
