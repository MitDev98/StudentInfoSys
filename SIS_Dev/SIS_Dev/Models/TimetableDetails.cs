using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIS_Dev.Models
{
    public class TimetableDetails
    {
        [Key]
        public int TimetabledetailsID { get; set; }
        public int TimetableID { get; set;}

        [Required]
        public bool Monday { get; set;}
        [Required]
        public bool Tuesday { get; set;}
        [Required]
        public bool Wednesday { get; set;}
        [Required]
        public bool Thursday { get; set;}
        [Required]
        public bool Friday { get; set;}
        [Required]
        public bool Saturday { get;set;}
        [Required]
        public bool AllDay { get; set;}
        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set;}
        public int CourseID { get; set; }
        public int FacultyID { get; set; }
    }
}
