namespace MailRuEncryptedCloud.DB
{
    using System;

    using MailRuEncryptedCloud.DB.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;

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

        public DbSet<Device> Devices { get; set; }

        public DbSet<DeviceRootFolder> DeviceRootFolders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlite(this.connectionString);
            builder.UseLazyLoadingProxies();
            builder.ConfigureWarnings(x => x.Throw(RelationalEventId.QueryClientEvaluationWarning));
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
                .HasMany(f => f.AllFolders)
                .WithOne(f => f.Root)
                .HasForeignKey(f => f.RootId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EncryptedFolder>()
                .HasMany(f => f.Files)
                .WithOne(f => f.Folder)
                .IsRequired()
                .HasForeignKey(f => f.FolderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EncryptedFolder>()
                .HasMany(f => f.DeviceRootFolders)
                .WithOne(f => f.Folder)
                .IsRequired()
                .HasForeignKey(f => f.FolderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Device>()
                .HasMany(f => f.DeviceRootFolders)
                .WithOne(f => f.Device)
                .IsRequired()
                .HasForeignKey(f => f.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Device>().HasIndex(d => d.DeviceName).IsUnique();

            modelBuilder.Entity<DeviceRootFolder>().HasKey(x => new { x.DeviceId, x.FolderId });
        }
    }
}
