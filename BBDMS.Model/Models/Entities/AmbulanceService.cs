using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class AmbulanceService
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string ProviderName { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        [Required]
        [StringLength(100)]
        public string AmbulanceType { get; set; } = "Basic Life Support (BLS)";

        [StringLength(50)]
        public string VehicleNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string ContactNumber { get; set; }

        public bool IsAvailable { get; set; } = true;

        [StringLength(50)]
        public string ServiceHours { get; set; } = "24/7";

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
