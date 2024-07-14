using System;
using Microsoft.EntityFrameworkCore;
using Plan_It.Models;
using Task = Plan_It.Models.Task;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;



namespace Plan_It.Data
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<OtpEntry> OtpEntries { get; set; }
        public DbSet<GroupTask> GroupTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure enums to be stored as strings
            modelBuilder.Entity<Task>()
                .Property(t => t.Priority)
                .HasConversion<string>();

            modelBuilder.Entity<Task>()
                .Property(t => t.Category)
                .HasConversion<string>();

            modelBuilder.Entity<Task>()
                .Property(t => t.TaskStatus)
                .HasConversion<string>();

            modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Group)
            .WithMany(g => g.Users)
            .HasForeignKey(u => u.GroupId);
           
           // Configure many-to-many relationship between Group and Task using the join entity
            modelBuilder.Entity<GroupTask>()
                .HasKey(gt => new { gt.GroupId, gt.TaskId });

            modelBuilder.Entity<GroupTask>()
                .HasOne(gt => gt.Group)
                .WithMany(g => g.GroupTasks)
                .HasForeignKey(gt => gt.GroupId);

            modelBuilder.Entity<GroupTask>()
                .HasOne(gt => gt.Task)
                .WithMany(t => t.GroupTasks)
                .HasForeignKey(gt => gt.TaskId);

        }
    }
}
