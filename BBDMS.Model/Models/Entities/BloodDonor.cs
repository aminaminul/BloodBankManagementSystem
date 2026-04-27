using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class BloodDonor
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string FullName { get; set; }
        [StringLength(11)]
        public string MobileNumber { get; set; }
        [StringLength(100)]
        public string EmailId { get; set; }
        [StringLength(20)]
        public string Gender { get; set; }
        public int? Age { get; set; }
        [StringLength(20)]
        public string BloodGroup { get; set; }
        [StringLength(255)]
        public string Address { get; set; }
        public string Message { get; set; }
        public DateTime PostingDate { get; set; } = DateTime.Now;
        public int? Status { get; set; }
        [StringLength(250)]
        public string Password { get; set; }
    }
}
