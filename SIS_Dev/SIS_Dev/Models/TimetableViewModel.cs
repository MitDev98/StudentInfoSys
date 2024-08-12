namespace SIS_Dev.Models
    {
    public class TimetableViewModel
        {

        public int TimetableID { get; set; }
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public int FacultyID { get; set; }
        public string FacultyName { get; set; }
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
        public int StandardID { get; set; }
        public string StandardName { get; set; }
        public string Days { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? InstituteID { get; set; }
        public string Name { get; set; }
        public string Year { get; set; }

        }
    }
