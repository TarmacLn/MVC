using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVC.Data
{
    public class UniversityAppDB : DbContext
    {
        public UniversityAppDB(DbContextOptions<UniversityAppDB> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Secretary> Secretaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<User>()
                .Property(u => u.UserType)
                .HasConversion<string>();

            modelBuilder.Entity<Student>().ToTable("students");
            modelBuilder.Entity<Student>()
                .HasKey(s => s.UserId);
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Professor>().ToTable("professors");
            modelBuilder.Entity<Professor>()
                .HasKey(p => p.UserId);
            modelBuilder.Entity<Professor>()
                .HasOne(p => p.User)
                .WithOne(u => u.Professor)
                .HasForeignKey<Professor>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Secretary>().ToTable("secretaries");
            modelBuilder.Entity<Secretary>()
                .HasKey(s => s.UserId);
            modelBuilder.Entity<Secretary>()
                .HasOne(s => s.User)
                .WithOne(u => u.Secretary)
                .HasForeignKey<Secretary>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

