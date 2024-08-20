using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class Timetable
        {
        [Key]
        public int TimetableID { get; set; }


        public int CourseID { get; set; }


        public int FacultyID { get; set; }

        public int SubjectID { get; set; }

        public int StandardID { get; set; }

        [Required(ErrorMessage = "Days are required")]
        public string Days { get; set; }

        [Required(ErrorMessage = "Start Time is required")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End Time is required")]
        public TimeSpan EndTime { get; set; }
        public DateTime? CreatedBy { get; set; }
        public DateTime? ModifiedBy { get; set; }

        public int InstituteID { get; set; }
        public string Year { get; set; }

        }
    }
