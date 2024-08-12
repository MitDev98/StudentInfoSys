namespace SIS_Dev.Models
    {
    public class AssessmentStatisticsViewModel
        {
        public int TotalStudents { get; set; }
        public int PassedStudents { get; set; }
        public int FailedStudents { get; set; }
        public int AbsentStudents { get; set; }
        public double AverageMarks { get; set; }
        }
    }
