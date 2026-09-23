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
        public string Name { get; set; }

        [StringLength(250)]
        public string HospitalAffiliation { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [StringLength(50)]
        public string ContactNumber { get; set; }

        [StringLength(100)]
        public string OperatingHours { get; set; } = "24/7";

        [StringLength(250)]
        public string AvailableBloodGroups { get; set; } = "A+, A-, B+, B-, AB+, AB-, O+, O-";

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
