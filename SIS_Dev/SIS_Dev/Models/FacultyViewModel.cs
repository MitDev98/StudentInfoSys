namespace SIS_Dev.Models
    {
    public class FacultyViewModel
        {

        public List<Faculty> Faculties { get; set; }
        public Dictionary<int, string> Institutes { get; set; }
        public string UserRole { get; set; }
        }
    }
