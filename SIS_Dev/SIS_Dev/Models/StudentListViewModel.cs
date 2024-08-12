namespace SIS_Dev.Models
    {
    public class StudentListViewModel
        {
        public int AssessmentId { get; set; }
        public List<Student> Students { get; set; }
        public List<AssessmentResult> AssessmentResults { get; set; }

        public StudentListViewModel()
            {
            Students = new List<Student>();
            AssessmentResults = new List<AssessmentResult>();
            }
        }
    }
