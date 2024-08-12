using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIS_Dev.Models
{
    public class Counsellor
    {
        [Key]
        public int CounsellorID { get; set; }

        public int UID { get; set; }

    }
}
