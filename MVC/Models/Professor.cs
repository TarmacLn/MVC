using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    public class Professor
    {
        [Column("user_id")]
        [Required]
        public int UserId { get; set; }

        [Column("afm")]
        [Required]
        public string AFM { get; set; } = null!;

        [Column("fullname")]
        [Required]
        public string Fullname { get; set; } = null!;

        [Column("department")]
        [Required]
        public string Department { get; set; } = null!;

        // Navigation property
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
