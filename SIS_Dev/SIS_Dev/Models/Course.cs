using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class Course
        {

        [Key]
        public int CourseID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
        public string Name { get; set; }

        [Required]
        public string Code { get; set; }
        public DateTime? CreatedBy { get; set; }
        public DateTime? ModifiedBy { get; set; }

        public int InstituteID { get; set; }
        public string Year { get; set; }


        }
    }
