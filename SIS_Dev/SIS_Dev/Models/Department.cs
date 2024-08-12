using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class Department
        {
        [Key]
        public int DID { get; set; }

        [Required]
        public string DName { get; set; }

        [Required]
        public string Code { get; set; }
        public DateTime? CreatedBy { get; set; }
        public DateTime? ModifiedBy { get; set; }

        }
    }
