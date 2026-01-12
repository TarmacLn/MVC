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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<User>()
                .Property(u => u.UserType)
                .HasConversion<string>();
        }
    }
}

