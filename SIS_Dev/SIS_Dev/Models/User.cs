using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class User
        {
        [Key]
        public int UID { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public DateTime? CreatedBy { get; set; }
        public DateTime? ModifiedBy { get; set; }

        }
    }
