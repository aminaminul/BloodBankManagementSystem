using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class BloodRequest
    {
        [Key]
        public int ID { get; set; }
        public int? BloodDonorID { get; set; }
        [StringLength(250)]
        public string Name { get; set; }
        [StringLength(250)]
        public string EmailId { get; set; }
        public long? ContactNumber { get; set; }
        [StringLength(250)]
        public string BloodRequireFor { get; set; }
        [StringLength(20)]
        public string? BloodGroup { get; set; }
        public int UnitsRequired { get; set; } = 1;
        [StringLength(250)]
        public string? HospitalName { get; set; }
        [StringLength(250)]
        public string? Location { get; set; }
        [StringLength(50)]
        public string? Urgency { get; set; } = "Urgent";
        [StringLength(50)]
        public string? Status { get; set; } = "Pending";
        public string Message { get; set; }
        public DateTime? ApplyDate { get; set; } = DateTime.Now;
    }
}
