using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class Hospital
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [StringLength(50)]
        public string ContactNumber { get; set; }

        [StringLength(50)]
        public string EmergencyHelpline { get; set; }

        [StringLength(500)]
        public string EmergencyServices { get; set; }

        [Display(Name = "Total Emergency Beds")]
        public int TotalEmergencyBeds { get; set; }

        [Display(Name = "Available Emergency Beds")]
        public int AvailableEmergencyBeds { get; set; }

        [Display(Name = "Total ICU Beds")]
        public int TotalIcuBeds { get; set; }

        [Display(Name = "Available ICU Beds")]
        public int AvailableIcuBeds { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
