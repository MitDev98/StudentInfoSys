using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class AttendanceViewModel
        {
        public int AttendanceID { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? SubjectName { get; set; }
        public int FacultyID { get; set; }
        public int StudentID { get; set; }
        [Required]
        public bool Status { get; set; }
        public string? Topic { get; set; }
        public string? StudentFirstName { get; set; }
        public string? StudentLastName { get; set; }

        // Edit URL for each attendance record
        public string? EditUrl { get; set; }
        }


    }
