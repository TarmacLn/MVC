using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MVC.Extensions;
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
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseHasStudent> CourseHasStudents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Department converter for display and storage
            var departmentConverter = new ValueConverter<Department, string>(
                v => v.ToDbValue(),
                v => DepartmentExtensions.FromDbValue(v)
            );

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<User>()
                .Property(u => u.UserType)
                .HasConversion<string>();

            modelBuilder.Entity<Student>().ToTable("students");
            modelBuilder.Entity<Student>()
                .HasKey(s => s.UserId);
            modelBuilder.Entity<Student>()
                .Property(s => s.Department)
                .HasConversion(departmentConverter);
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Professor>().ToTable("professors");
            modelBuilder.Entity<Professor>()
                .HasKey(p => p.UserId);
            modelBuilder.Entity<Professor>()
                .Property(p => p.Department)
                .HasConversion(departmentConverter);
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

            modelBuilder.Entity<Course>().ToTable("courses");
            modelBuilder.Entity<Course>()
                .HasKey(c => c.CourseId);
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Professor)
                .WithMany()
                .HasForeignKey(c => c.ProfessorId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<CourseHasStudent>().ToTable("course_has_student");
            modelBuilder.Entity<CourseHasStudent>()
                .HasKey(chs => chs.Id);
            modelBuilder.Entity<CourseHasStudent>()
                .HasOne(chs => chs.Course)
                .WithMany(c => c.CourseHasStudents)
                .HasForeignKey(chs => chs.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<CourseHasStudent>()
                .HasOne(chs => chs.Student)
                .WithMany(s => s.EnrolledCourses)
                .HasForeignKey(chs => chs.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

