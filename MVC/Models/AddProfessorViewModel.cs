using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
    public class AddProfessorViewModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Display(Name = "AFM")]
        public string AFM { get; set; } = string.Empty;

        
        [Required]
        [Display(Name = "Full Name")]
        public string Fullname { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Department")]
        public Department Department { get; set; }
    }
}