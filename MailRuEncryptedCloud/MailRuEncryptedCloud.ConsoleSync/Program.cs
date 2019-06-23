namespace MailRuEncryptedCloud.ConsoleSync
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MailRuEncryptedCloud.DB;
    using MailRuEncryptedCloud.DB.Models;

    using Microsoft.EntityFrameworkCore;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            using (var db = new DirDbContext("temp1.db", null))
            {
                db.Database.Migrate();
                var folder = new EncryptedFolder
                                 {
                                     Name = "First folder",
                                     Created = DateTime.Now,
                                     EncryptedName = "First folder encrypted",
                                     Files = new List<EncryptedFile>
                                                 {
                                                     new EncryptedFile
                                                         {
                                                             Name = "First file"
                                                         },
                                                     new EncryptedFile
                                                         {
                                                             Name = "Second file"
                                                         }
                                                 }
                                 };
                db.Folders.Add(folder);
                await db.SaveChangesAsync();
            }

            Console.WriteLine("End");
            Console.ReadKey();
        }
    }
}
