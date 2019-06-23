namespace MailRuEncryptedCloud.Sync
{
    using System.Linq;
    using System.Threading.Tasks;

    using MailRuEncryptedCloud.DB;
    using MailRuEncryptedCloud.Encryption;

    using Microsoft.EntityFrameworkCore;

    public class Backuper
    {
        private readonly DirDbContext db;
        private readonly IEncryptor encryptor;

        public Backuper(DirDbContext db, IEncryptor encryptor)
        {
            this.db = db;
            this.encryptor = encryptor;
        }

        public async Task UploadFolder(string folder)
        {
            var existingFolders = await this.db.Folders
                                      .Where(f => f.ParentId == null && f.Name == folder)
                                      .Include(f => f.Files).ToListAsync();

        }
    }
}
