namespace MailRuEncryptedCloud.Tests
{
    using MailRuEncryptedCloud.DB;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;

    internal class TestDbContext : DirDbContext
    {
        private const string InMemoryDbName = "DirInMemoryDb";

        public TestDbContext()
            : base(string.Empty, string.Empty)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseInMemoryDatabase(InMemoryDbName);
            builder.UseLazyLoadingProxies();
            builder.ConfigureWarnings(x => x.Throw(RelationalEventId.QueryClientEvaluationWarning));
        }
    }
}
