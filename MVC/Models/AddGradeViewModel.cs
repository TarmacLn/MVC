using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
    public class AddGradeViewModel
    {
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grade is required")]
        [Range(0, 10, ErrorMessage = "Grade must be between 0 and 10")]
        public int Grade { get; set; }

        [StringLength(60, ErrorMessage = "Description cannot exceed 60 characters")]
        public string? Description { get; set; }

        [Display(Name = "Is Final Grade")]
        public bool IsFinal { get; set; } = false;
    }
}
