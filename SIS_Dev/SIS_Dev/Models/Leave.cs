using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class Leave
        {
        [Key]
        public int LeaveID { get; set; }


        public int StudentID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; }

        [Required]
        public int FacultyID { get; set; }

        [Required]
        public int SubjectID { get; set; }

        [Required]
        [StringLength(50)]
        public string LeaveType { get; set; }

        public string Status { get; set; } // Pending, Approved, Rejected
        }
    }
