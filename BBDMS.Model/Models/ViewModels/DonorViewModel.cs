using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.ViewModels
{
    public class DonorViewModel
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [StringLength(11)]
        public string MobileNumber { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string EmailId { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string BloodGroup { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
