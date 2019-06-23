namespace MailRuEncryptedCloud.DB
{
    using System;

    using MailRuEncryptedCloud.DB.Models;

    using Microsoft.EntityFrameworkCore;

    public class DirDbContext : DbContext
    {
        private readonly string connectionString;

        public DirDbContext(string dbFilePath, string dbPassword)
        {
            this.connectionString = $"Data Source={dbFilePath};"
                                    + (string.IsNullOrEmpty(dbPassword) ? string.Empty : $"Password={dbPassword};");
        }

        [Obsolete("For migration creation only")]
        public DirDbContext()
        {
            this.connectionString = "Data Source=temp.db;Version=3;";
        }

        public DbSet<EncryptedFile> Files { get; set; }

        public DbSet<EncryptedFolder> Folders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlite(this.connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EncryptedFolder>()
                        .HasMany(f => f.Folders)
                        .WithOne(f => f.Parent)
                        .HasForeignKey(f => f.ParentId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EncryptedFolder>()
                        .HasMany(f => f.Files)
                        .WithOne(f => f.Folder)
                        .IsRequired()
                        .HasForeignKey(f => f.FolderId)
                        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
