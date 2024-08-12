using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class Standard
        {

        [Key]
        public int StandardID { get; set; }

        [Required(ErrorMessage = "Standard Name is required.")]
        [StringLength(50, ErrorMessage = "Standard Name cannot exceed {1} characters.")]
        public string StandardName { get; set; }

        public int CourseID { get; set; }
        public DateTime? CreatedBy { get; set; }
        public DateTime? ModifiedBy { get; set; }
        public int InstituteID { get; set; }
        public string Year { get; set; }


        }
    }
