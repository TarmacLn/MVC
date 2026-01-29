using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    public class CourseHasStudent
    {
        [Column("course_id")]
        public int CourseId { get; set; }

        [Column("student_id")]
        public int StudentId { get; set; }

        [Column("grade")]
        public int? Grade { get; set; }

        [Column("is_final")]
        public bool? IsFinal { get; set; } = false;

        [Column("description")]
        [MaxLength(60)]
        public string? Description { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; } = null!;

        [ForeignKey("StudentId")]
        public Student Student { get; set; } = null!;
    }
}