using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    public class Student
    {
        [Column("user_id")]
        [Required]
        public int UserId { get; set; }

        [Column("fullname")]
        [Required]
        public string Fullname { get; set; } = null!;

        [Column("department")]
        [Required]
        public Department Department { get; set; } = Department.ComputerScience;

        // Navigation property
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public ICollection<CourseHasStudent> EnrolledCourses { get; set; } = new List<CourseHasStudent>();
    }
}
