using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class Institute
        {
        [Key]
        public int InstituteID { get; set; }

        [Required]
        public string Name { get; set; }
        public int AdminID { get; set; }
        public DateTime? CreatedBy { get; set; }
        public DateTime? ModifiedBy { get; set; }
        }
    }