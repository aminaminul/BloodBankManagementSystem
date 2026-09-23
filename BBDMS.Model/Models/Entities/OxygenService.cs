using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class OxygenService
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string ProviderName { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        [StringLength(50)]
        public string ContactNumber { get; set; }

        [StringLength(50)]
        public string OxygenAvailability { get; set; } = "In Stock"; // "In Stock", "Limited", "Out of Stock"

        [StringLength(100)]
        public string CylinderCapacity { get; set; } = "1.4m³, 6.8m³, Concentrators";

        public bool HomeDeliveryAvailable { get; set; } = true;

        [StringLength(500)]
        public string ServiceDetails { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
