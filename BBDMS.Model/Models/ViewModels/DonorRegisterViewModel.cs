using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.ViewModels
{
    /// <summary>
    /// Everything a new donor may submit. Binding the <c>BloodDonor</c> entity directly allowed a
    /// crafted form to post <c>Id</c>, <c>Status</c>, <c>LastDonatedDate</c> (fake eligibility) and
    /// <c>ThemePreference</c>; this shape simply has nowhere to put them.
    /// </summary>
    public class DonorRegisterViewModel
    {
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

        [Required(ErrorMessage = "Please tell requesters when you can donate.")]
        [StringLength(300)]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;
    }
}
