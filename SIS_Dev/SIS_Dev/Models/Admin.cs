using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class Admin
        {
        [Key]
        public int AdminID { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public int InstituteID { get; set; }
        }
    }
