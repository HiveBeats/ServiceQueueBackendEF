using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebApi.Models
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext()
        {

        }

        public AppDbContext(DbContextOptions options) : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            AppDbContextBuilder.ConfigureModels(modelBuilder);
            AppDbContextBuilder.ConfigureIdentityModels(modelBuilder);
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Script> Scripts { get; set; }
        public DbSet<ScriptLog> ScriptLogs { get; set; }
    }
}