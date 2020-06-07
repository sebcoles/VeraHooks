using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<PropertyCondition> Conditons { get; set; }
        public DbSet<MitigationWebhook> WebHooks { get; set; }
        public DbSet<WebhookLog> Logs { get; set; }
        public DbSet<App> Apps { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {}
    }
}
