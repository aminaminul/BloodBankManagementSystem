using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.ViewModels
{
    public class DonorViewModel
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(11)]
        public string MobileNumber { get; set; }

        [Required]
        [EmailAddress]
        public string EmailId { get; set; }

        public string Gender { get; set; }
        public int? Age { get; set; }
        public string BloodGroup { get; set; }
        public string Address { get; set; }
        public string Message { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
