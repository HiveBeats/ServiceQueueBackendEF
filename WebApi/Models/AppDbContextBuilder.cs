using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models
{
    public static class AppDbContextBuilder
    {
        public static void ConfigureModels(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>().HasKey(t => new {t.AppUserId, t.TokenString});
            modelBuilder.Entity<RefreshToken>().HasIndex(t => t.ExpireAt);
            
            modelBuilder.Entity<Service>().HasOne(x=> x.Parent)
                                        .WithMany(x=> x.Childs)
                                        .HasForeignKey(x=> x.ParentId)
                                        .IsRequired(false)
                                        .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Topic>().HasIndex(t => t.Name).IsUnique();

            modelBuilder.Entity<Message>().HasIndex(m => m.DateSolved);

            modelBuilder.Entity<Script>().HasIndex(s => s.Name).IsUnique();
            modelBuilder.Entity<Script>().HasIndex(s => s.Priority);
            modelBuilder.Entity<Script>().HasIndex(s => s.IsEnabled);
            modelBuilder.Entity<Script>().Property(s => s.LogLevel).HasConversion<int>();
            
            modelBuilder.Entity<ScriptLog>().HasIndex(sl => sl.LogLevel);
            modelBuilder.Entity<ScriptLog>().Property(sl => sl.LogLevel).HasConversion<int>();
        }

        public static void ConfigureIdentityModels(ModelBuilder builder)
        {
            builder.Entity<AppUser>(entity => entity.Property(m => m.Id).HasMaxLength(128));
            builder.Entity<AppUser>(entity => entity.Property(m => m.NormalizedEmail).HasMaxLength(128));
            builder.Entity<AppUser>(entity => entity.Property(m => m.NormalizedUserName).HasMaxLength(128));

            builder.Entity<IdentityRole>(entity => entity.Property(m => m.Id).HasMaxLength(128));
            builder.Entity<IdentityRole>(entity => entity.Property(m => m.NormalizedName).HasMaxLength(128));

            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(128));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.ProviderKey).HasMaxLength(128));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(128));
            
            builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(128));
            builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(128));

            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(128));
            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(128));
            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.Name).HasMaxLength(128));

            builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(128));
            
            builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(128));
        }
    }
}