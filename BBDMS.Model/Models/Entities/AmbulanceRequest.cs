using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class AmbulanceRequest
    {
        [Key]
        public int Id { get; set; }

        public int? AmbulanceServiceId { get; set; }

        [Required]
        [StringLength(250)]
        public string PatientName { get; set; }

        [Required]
        [StringLength(50)]
        public string ContactNumber { get; set; }

        [Required]
        [StringLength(500)]
        public string PickupAddress { get; set; }

        [StringLength(250)]
        public string DestinationHospital { get; set; }

        [StringLength(100)]
        public string AmbulanceType { get; set; } = "Basic Life Support (BLS)";

        [StringLength(50)]
        public string Urgency { get; set; } = "Emergency";

        public DateTime RequestDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public string Notes { get; set; }
    }
}
