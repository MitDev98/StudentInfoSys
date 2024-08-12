using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace SIS_Dev.Models
    {
    public class Role
        {
        [Key]
        public int RoleID { get; set; }

        [Required]
        public string RoleName { get; set; }
        public DateTime? CreatedBy { get; set; }
        public DateTime? ModifiedBy { get; set; }

        }
    }
