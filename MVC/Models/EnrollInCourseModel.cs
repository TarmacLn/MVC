using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
     public class EnrollInCourseViewModel
    {
        [Required(ErrorMessage = "Please select a course")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required]
        public int StudentId { get; set; }

        // Only for display
        public string StudentName { get; set; } = string.Empty;
        public string StudentDepartment { get; set; } = string.Empty;

        // For the dropdowns
        public SelectList? Courses { get; set; }
    }
}