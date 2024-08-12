using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class Assessment
        {

        [Key]
        public int AssessmentID { get; set; }


        public string SubjectName { get; set; }

        [Required(ErrorMessage = "Start Time is required.")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End Time is required.")]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }


        public double TotalMarks { get; set; }


        public double PassingMarks { get; set; }

        public int StandardID { get; set; }

        public int CourseID { get; set; }

        public int FacultyID { get; set; }
        public DateTime? CreatedBy { get; set; }
        public DateTime? ModifiedBy { get; set; }

        public int InstituteID { get; set; }


        }
    }
