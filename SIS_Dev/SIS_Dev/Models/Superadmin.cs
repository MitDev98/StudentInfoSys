using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class Superadmin
        {

        [Key]
        public int SuperAdminID { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }


        }
    }
