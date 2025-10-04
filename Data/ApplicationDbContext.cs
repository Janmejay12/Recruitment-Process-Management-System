using Microsoft.EntityFrameworkCore;
using Recruitment_System.Entities;

namespace Recruitment_System.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            SeedData(modelBuilder);

        }



        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                    new Role { RoleId = 1, RoleName = "Admin" },
                    new Role { RoleId = 2, RoleName = "Recruiter" },
                    new Role { RoleId = 3, RoleName = "HR" },
                    new Role { RoleId = 4, RoleName = "Interviewer" },
                    new Role { RoleId = 5, RoleName = "Reviewer" },
                    new Role { RoleId = 6, RoleName = "Candidate" },
                    new Role { RoleId = 7, RoleName = "Viewer" }
            );

            modelBuilder.Entity<User>().HasData(
                 new User
                 {
                     UserId = 1,
                     FullName = "System Administrator",
                     Email = "admin@recruitment.com",
                     PasswordHash = "$2a$12$teJFviM61rhCz8dzSWgYIuhWug2TblSyuFgn6yYOEQhELKnKoc2I6",
                     Status = "Active"
                 }
            );

            modelBuilder.Entity<UserRole>().HasData(
                    new UserRole { UserId = 1, RoleId = 1 }
            );
        }
    }
}
