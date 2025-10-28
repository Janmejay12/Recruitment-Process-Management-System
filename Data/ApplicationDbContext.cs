using Microsoft.EntityFrameworkCore;
using Recruitment_System.Entities;

namespace Recruitment_System.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }
        public DbSet<JobClosure> JobClosures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            ConfigureUserAndRoles(modelBuilder);

            // Configure Job-related entities
            ConfigureJob(modelBuilder);
            ConfigureSkill(modelBuilder);
            ConfigureJobSkill(modelBuilder);
            ConfigureJobClosure(modelBuilder);

            // Seed data
            SeedData(modelBuilder);
        }

        private void ConfigureUserAndRoles(ModelBuilder modelBuilder)
        {
            // UserRole composite key
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // UserRole relationships
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureJob(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>()
                .ToTable("Jobs");

            modelBuilder.Entity<Job>()
                .HasKey(j => j.JobId);

            // Foreign key to User (CreatedBy)
            modelBuilder.Entity<Job>()
                .HasOne(j => j.CreatedByUser)
                .WithMany()
                .HasForeignKey(j => j.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship with JobSkills
            modelBuilder.Entity<Job>()
                .HasMany(j => j.JobSkills)
                .WithOne(js => js.Job)
                .HasForeignKey(js => js.JobId)
                .OnDelete(DeleteBehavior.Cascade); // Delete skills when job is deleted

            // Relationship with JobClosures
            modelBuilder.Entity<Job>()
                .HasMany(j => j.JobClosures)
                .WithOne(jc => jc.Job)
                .HasForeignKey(jc => jc.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // Default values
            modelBuilder.Entity<Job>()
                .Property(j => j.Status)
                .HasDefaultValue("Open");

            modelBuilder.Entity<Job>()
                .Property(j => j.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Job>()
                .Property(j => j.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Job>()
                .Property(j => j.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }

        private void ConfigureSkill(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>()
                .ToTable("Skills");

            modelBuilder.Entity<Skill>()
                .HasKey(s => s.SkillId);

            // Unique constraint on SkillName
            modelBuilder.Entity<Skill>()
                .HasIndex(s => s.SkillName)
                .IsUnique();

            // Relationship with JobSkills
            modelBuilder.Entity<Skill>()
                .HasMany(s => s.JobSkills)
                .WithOne(js => js.Skill)
                .HasForeignKey(js => js.SkillId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete skill if used in jobs

            // Default value
            modelBuilder.Entity<Skill>()
                .Property(s => s.IsActive)
                .HasDefaultValue(true);
        }

        private void ConfigureJobSkill(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobSkill>()
                .ToTable("JobSkills");

            modelBuilder.Entity<JobSkill>()
                .HasKey(js => js.JobSkillId);

            modelBuilder.Entity<JobSkill>()
                .HasIndex(js => new { js.JobId, js.SkillId })
                .IsUnique();


            // Default values
            modelBuilder.Entity<JobSkill>()
                .Property(js => js.IsMandatory)
                .HasDefaultValue(true);

            modelBuilder.Entity<JobSkill>()
                .Property(js => js.Priority)
                .HasDefaultValue(1);
        }

        private void ConfigureJobClosure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobClosure>()
                .ToTable("JobClosures");

            modelBuilder.Entity<JobClosure>()
                .HasKey(jc => jc.ClosureId);


            // Foreign key to User (ClosedBy)
            modelBuilder.Entity<JobClosure>()
                .HasOne(jc => jc.ClosedByUser)
                .WithMany()
                .HasForeignKey(jc => jc.ClosedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Foreign key to Candidate (SelectedCandidateId) - nullable
            modelBuilder.Entity<JobClosure>()
                .HasOne(jc => jc.SelectedCandidate)
                .WithMany()
                .HasForeignKey(jc => jc.SelectedCandidateId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Default value
            modelBuilder.Entity<JobClosure>()
                .Property(jc => jc.ClosedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "Recruiter" },
                new Role { RoleId = 3, RoleName = "HR" },
                new Role { RoleId = 4, RoleName = "Interviewer" },
                new Role { RoleId = 5, RoleName = "Reviewer" },
                new Role { RoleId = 6, RoleName = "Candidate" },
                new Role { RoleId = 7, RoleName = "Viewer" }
            );

            // Seed Admin User
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

            // Seed Admin Role Assignment
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserId = 1, RoleId = 1 }
            );

            // Seed some common skills
            modelBuilder.Entity<Skill>().HasData(
                new Skill { SkillId = 1, SkillName = "C#", Category = "Programming", SkillLevel = "Advanced", IsActive = true },
                new Skill { SkillId = 2, SkillName = "ASP.NET Core", Category = "Framework", SkillLevel = "Advanced", IsActive = true },
                new Skill { SkillId = 3, SkillName = "React", Category = "Frontend", SkillLevel = "Intermediate", IsActive = true },
                new Skill { SkillId = 4, SkillName = "SQL Server", Category = "Database", SkillLevel = "Intermediate", IsActive = true },
                new Skill { SkillId = 5, SkillName = "JavaScript", Category = "Programming", SkillLevel = "Intermediate", IsActive = true },
                new Skill { SkillId = 6, SkillName = "Entity Framework", Category = "ORM", SkillLevel = "Advanced", IsActive = true },
                new Skill { SkillId = 7, SkillName = "Git", Category = "Version Control", SkillLevel = "Beginner", IsActive = true },
                new Skill { SkillId = 8, SkillName = "Azure", Category = "Cloud", SkillLevel = "Advanced", IsActive = true }
            );
        }
    }
}
