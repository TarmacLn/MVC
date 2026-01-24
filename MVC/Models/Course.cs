using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{

    public class Course
    {
        [Column("course_id")]
        public int CourseId { get; set; }

        [Column("course_title")]
        [Required]
        public string Title { get; set; } = null!;

        [Column("course_semester")]
        [Required]
        public int Semester { get; set; } 

        [Column("professor_id")]
        public int? ProfessorId { get; set; }

        [ForeignKey("ProfessorId")]
        public Professor? Professor { get; set; } 
    }
}