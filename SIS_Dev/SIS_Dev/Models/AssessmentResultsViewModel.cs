namespace SIS_Dev.Models
    {
    public class AssessmentResultsViewModel
        {
        public int AssessmentId { get; set; }
        public List<AssessmentResult> AssessmentResults { get; set; }
        public List<Student> Students { get; set; }

        public int TotalStudents => Students.Count;
        public int TotalPassed => AssessmentResults.Count(ar => !ar.Absent && ar.ObtainedMarks >= 40);
        public int TotalFailed => AssessmentResults.Count(ar => !ar.Absent && ar.ObtainedMarks < 40);
        public int TotalAbsent => AssessmentResults.Count(ar => ar.Absent);

        public double AverageMarks => AssessmentResults
            .Where(ar => !ar.Absent)
            .Average(ar => ar.ObtainedMarks) ?? 0;

        public string GetMarksCategory(double? marks)
            {
            if (!marks.HasValue)
                {
                return "No Marks";
                }

            if (marks < 40)
                {
                return "Fail";
                }
            else if (marks >= 40 && marks < 60)
                {
                return "Pass";
                }
            else if (marks >= 60 && marks < 80)
                {
                return "Good";
                }
            else if (marks >= 80)
                {
                return "Very Good";
                }
            return string.Empty;
            }
        }
    }
