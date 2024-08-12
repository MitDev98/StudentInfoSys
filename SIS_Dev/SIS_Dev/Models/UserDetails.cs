using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class UserDetails
        {
        [Key]
        public int UserdetailsID { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Middle Name is required.")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        public DateTime Date_of_birth { get; set; }

        [Required(ErrorMessage = "Contact number is required.")]
        public string ContactSelf { get; set; }

        [Required(ErrorMessage = "Reference Contact number is required.")]
        public string ContactRef { get; set; }

        public int UID { get; set; }

        [Required(ErrorMessage = "Current Address is required.")]
        public string CurrentAddress { get; set; }

        [Required(ErrorMessage = "Current Pincode is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Current Pincode must be 6 digits.")]
        public string CurrentPincode { get; set; }

        [Required(ErrorMessage = "Permanent Address is required.")]
        public string PermanentAddress { get; set; }

        [Required(ErrorMessage = "Permanent Pincode is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Permanent Pincode must be 6 digits.")]
        public string PermanentPincode { get; set; }

        public DateTime? CreatedBy { get; set; }
        public DateTime? ModifiedBy { get; set; }

        }
    }
