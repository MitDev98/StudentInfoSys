using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class Student
        {

        [Key]
        public int StudentID { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed {1} characters.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed {1} characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Contact number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string Contactno { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(100, ErrorMessage = "Address cannot exceed {1} characters.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Pincode is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Invalid pincode ")]
        public string Pincode { get; set; }
        public string Year { get; set; }


        public int CourseID { get; set; }

        public int StandardID { get; set; }
        public DateTime? CreatedBy { get; set; }
        public DateTime? ModifiedBy { get; set; }

        public int InstituteID { get; set; }
        public int FacultyID { get; set; }


        }
    }
