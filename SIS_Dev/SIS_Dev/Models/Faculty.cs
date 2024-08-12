using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class Faculty
        {

        [Key]
        public int FacultyID { get; set; }


        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Contact number is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Invalid Contact Number.")]
        public string Contactno { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Pincode is required.")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Invalid Pincode.")]
        public string Pincode { get; set; }

        public DateTime? CreatedBy { get; set; }
        public DateTime? ModifiedBy { get; set; }
        public int InstituteID { get; set; }


        public int CourseID { get; set; }
        public int StandardID { get; set; }
        public string Year { get; set; }

        public int SubjectID { get; set; }

        }
    }