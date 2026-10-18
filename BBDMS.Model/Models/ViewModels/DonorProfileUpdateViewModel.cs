using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.ViewModels
{
    /// <summary>
    /// The only fields a donor may change on their own profile. Account identity
    /// (email, password, status) and medical eligibility (last donation date, which is
    /// derived from the recorded donation history) are deliberately excluded so they
    /// cannot be set from a posted form.
    /// </summary>
    public class DonorProfileUpdateViewModel
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile Number is required.")]
        [StringLength(15, ErrorMessage = "Mobile Number cannot exceed 15 digits.")]
        [RegularExpression(@"^[0-9+\-\s()]{7,15}$", ErrorMessage = "Please enter a valid phone number.")]
        public string MobileNumber { get; set; } = string.Empty;

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

        [StringLength(500)]
        public string? Message { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
