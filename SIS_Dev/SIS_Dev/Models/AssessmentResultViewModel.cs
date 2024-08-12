namespace SIS_Dev.Models
    {
    public class AssessmentResultViewModel
        {
        public int AssessmentResultID { get; set; }
        public int AssessmentID { get; set; }
        public int StudentID { get; set; }
        public double ObtainedMarks { get; set; }
        public string Remark { get; set; }
        public bool Absent { get; set; }
        public string StudentName { get; set; }
        public string SubjectName { get; set; }
        public DateTime DueDate { get; set; }
        public List<Student> Students { get; set; } = new List<Student>();
        public Assessment Assessment { get; set; }
        public int AssessmentId { get; set; }
        public List<AssessmentResult> AssessmentResults { get; set; }


        }
    }



