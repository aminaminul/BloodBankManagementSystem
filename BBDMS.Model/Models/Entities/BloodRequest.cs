using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class BloodRequest
    {
        [Key]
        public int ID { get; set; }

        public int? BloodDonorID { get; set; }

        [Required(ErrorMessage = "Requester name is required.")]
        [StringLength(250)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(250)]
        public string EmailId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required.")]
        public long? ContactNumber { get; set; }

        [Required(ErrorMessage = "Please specify whom the blood is required for.")]
        [StringLength(250)]
        public string BloodRequireFor { get; set; } = string.Empty;

        [StringLength(20)]
        public string? BloodGroup { get; set; }

        [Range(1, 100, ErrorMessage = "Units required must be at least 1.")]
        public int UnitsRequired { get; set; } = 1;

        [StringLength(250)]
        public string? HospitalName { get; set; }

        [StringLength(250)]
        public string? Location { get; set; }

        [StringLength(50)]
        public string? Urgency { get; set; } = "Urgent";

        [StringLength(50)]
        public string? Status { get; set; } = "Pending";

        [Required(ErrorMessage = "Please provide details or a message regarding the request.")]
        public string Message { get; set; } = string.Empty;

        public DateTime? ApplyDate { get; set; } = DateTime.Now;
    }
}
