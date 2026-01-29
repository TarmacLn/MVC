
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{

    public enum UserType
    {
        Secretary,
        Professor,
        Student
    }

    public enum Department
    {
        [Display(Name = "Computer Science")]
        ComputerScience,
        [Display(Name = "Business Administration")]
        BusinessAdministration,
        [Display(Name = "Sociology")]
        Sociology
    }

    public class User
    {
        [Column("user_id")]
        public int Id { get; set; }
        [Column("username")]
        [Required]
        public string Username { get; set; } = null!;
        [Column("user_password")]
        [Required]
        public string Password { get; set; } = null!;
        [Column("user_type")]
        [Required]
        public UserType UserType { get; set; } = UserType.Student;

        // Navigation properties
        public Student? Student { get; set; }
        public Professor? Professor { get; set; }
        public Secretary? Secretary { get; set; }
    }
}