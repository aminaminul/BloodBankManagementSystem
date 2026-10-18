using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class BloodBank
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; } = string.Empty;
        [StringLength(250)]
        public string HospitalAffiliation { get; set; } = string.Empty;
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;
        [StringLength(50)]
        public string ContactNumber { get; set; } = string.Empty;
        [StringLength(100)]
        public string OperatingHours { get; set; } = "24/7";

        [StringLength(250)]
        public string AvailableBloodGroups { get; set; } = "A+, A-, B+, B-, AB+, AB-, O+, O-";

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
