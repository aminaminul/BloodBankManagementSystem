using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class AmbulanceRequest
    {
        [Key]
        public int Id { get; set; }

        public int? AmbulanceServiceId { get; set; }

        /// <summary>
        /// Unguessable public reference for the tracking page. Sequential ids must never be used to
        /// look up a request: anyone could enumerate other patients' names and phone numbers.
        /// </summary>
        [StringLength(64)]
        public string? TrackingToken { get; set; }

        [Required]
        [StringLength(250)]
        public string PatientName { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string ContactNumber { get; set; } = string.Empty;
        [Required]
        [StringLength(500)]
        public string PickupAddress { get; set; } = string.Empty;
        // Optional in the booking form — must stay nullable, otherwise model binding
        // rejects the whole request when the patient leaves the field empty.
        [StringLength(250)]
        public string? DestinationHospital { get; set; }

        [StringLength(100)]
        public string AmbulanceType { get; set; } = "Basic Life Support (BLS)";

        [StringLength(50)]
        public string Urgency { get; set; } = "Emergency";

        public DateTime RequestDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
