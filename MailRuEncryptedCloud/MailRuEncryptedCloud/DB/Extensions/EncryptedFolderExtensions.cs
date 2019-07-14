namespace MailRuEncryptedCloud.DB.Extensions
{
    using System.IO;

    using MailRuEncryptedCloud.DB.Models;

    public static class EncryptedFolderExtensions
    {
        public static string GetRelativeEncryptedPath(this EncryptedFolder ecryptedFolder)
        {
            var path = ecryptedFolder.EncryptedName;
            while (ecryptedFolder.Parent != null)
            {
                ecryptedFolder = ecryptedFolder.Parent;
                path = Path.Combine(ecryptedFolder.EncryptedName, path);
            }

            return path;
        }

        public static string GetRelativePath(this EncryptedFolder ecryptedFolder)
        {
            var path = ecryptedFolder.Name;
            while (ecryptedFolder.Parent != null)
            {
                ecryptedFolder = ecryptedFolder.Parent;
                path = Path.Combine(ecryptedFolder.Name, path);
            }

            return path;
        }
    }
}
