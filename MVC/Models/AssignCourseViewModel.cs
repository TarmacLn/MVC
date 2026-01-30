using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
     public class AssignCourseViewModel
    {
        [Required(ErrorMessage = "Please select a course")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required]
        public int ProfessorId { get; set; }

        // Only for display
        public string ProfessorName { get; set; } = string.Empty;
        public Department ProfessorDepartment { get; set; } = Department.ComputerScience;

        // For the dropdowns
        public SelectList? Courses { get; set; }
    }
}