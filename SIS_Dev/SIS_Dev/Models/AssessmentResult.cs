using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class AssessmentResult
        {
        [Key]
        public int AssessmentResultID { get; set; }

        public int AssessmentID { get; set; }

        public int StudentID { get; set; }

        public bool Absent { get; set; }

        public double? ObtainedMarks { get; set; }

        public string? Remark { get; set; }
        public DateTime? CreatedBy { get; set; }
        public DateTime? ModifiedBy { get; set; }
        public int InstituteID { get; set; }
        //public int FacultyID { get; set; }

        }
    }
