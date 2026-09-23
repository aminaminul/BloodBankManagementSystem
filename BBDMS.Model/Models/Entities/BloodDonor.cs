using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class BloodDonor
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile Number is required.")]
        [StringLength(15, ErrorMessage = "Mobile Number cannot exceed 15 digits.")]
        [RegularExpression(@"^[0-9+\-\s()]{7,15}$", ErrorMessage = "Please enter a valid phone number.")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email Address is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address format.")]
        [StringLength(100)]
        public string EmailId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required.")]
        [Range(18, 65, ErrorMessage = "Donor must be between 18 and 65 years old.")]
        public int? Age { get; set; }

        [Required(ErrorMessage = "Blood Group is required.")]
        [StringLength(20)]
        public string BloodGroup { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(255)]
        public string Address { get; set; } = string.Empty;

        public string? Message { get; set; }

        public DateTime PostingDate { get; set; } = DateTime.Now;

        public int? Status { get; set; } = 1;

        public bool IsAvailable { get; set; } = true;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(250, MinimumLength = 4, ErrorMessage = "Password must be at least 4 characters.")]
        public string Password { get; set; } = string.Empty;

        // Medical Eligibility Tracking (90-day minimum between donations)
        public DateTime? LastDonatedDate { get; set; }

        public bool IsEligibleToDonate =>
            !LastDonatedDate.HasValue || (DateTime.Now - LastDonatedDate.Value).TotalDays >= 90;

        public int DaysUntilEligible =>
            !LastDonatedDate.HasValue ? 0 : Math.Max(0, 90 - (int)(DateTime.Now - LastDonatedDate.Value).TotalDays);
    }
}
