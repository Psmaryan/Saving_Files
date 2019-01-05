using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SavingFile.Models;

namespace CustomIdentityApp.Models
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<SaveFile> SaveFiles { get; set; }
        public DbSet<UserSaveFile> UserSaveFiles { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
          
            
            /*   modelBuilder.Entity<UserSaveFile>()
                   .Property(f => f.DateAdded)
                   .HasColumnType("datetime2");*/

            /*  modelBuilder.Entity<UserSaveFile>()
                  .HasKey(t => new { t.UserId, t.SaveFileId });*/

            /* modelBuilder.Entity<UserSaveFile>()
                 .HasOne(sc => sc.Users)
                 .WithMany(s => s.UserSaveFiles)
                 .HasForeignKey(sc => sc.UserId);*/

            /*      modelBuilder.Entity<UserSaveFile>()
                      .HasOne(sc => sc.SaveFiles)
                      .WithMany(c => c.UserSaveFiles)
                      .HasForeignKey(sc => sc.SaveFileId);*/
        }
    }
}