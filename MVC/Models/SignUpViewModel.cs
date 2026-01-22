using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
    public class SignUpViewModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "User type")]
        public UserType UserType { get; set; } = UserType.Student;

        // Student fields
        [Display(Name = "Full name")]
        public string? Fullname { get; set; }

        [Display(Name = "Department")]
        public Department? Department { get; set; }

        // Professor fields
        [Display(Name = "AFM")]
        public string? AFM { get; set; }

        // Secretary fields
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }
    }
}

