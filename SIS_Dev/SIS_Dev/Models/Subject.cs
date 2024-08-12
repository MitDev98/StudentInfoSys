using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class Subject
        {
        [Key]
        public int SubjectID { get; set; }

        [Required]
        public string SubjectCode { get; set; }

        [Required]
        public string SubjectName { get; set; }

        public int CourseID { get; set; }

        public int StandardID { get; set; }
        public DateTime? CreatedBy { get; set; }
        public DateTime? ModifiedBy { get; set; }

        public string Year { get; set; }

        public int InstituteID { get; set; }
        }
    }
