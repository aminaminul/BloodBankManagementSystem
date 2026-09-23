using BBDMS.Model.Models.Entities;

namespace BBDMS.Model.Models.ViewModels
{
    public class HeaderViewModel
    {
        public ContactInfo? ContactInfo { get; set; }
        public int? DonorId { get; set; }
        public int? AdminId { get; set; }
        public bool IsDonorLoggedIn => DonorId.HasValue;
        public bool IsAdminLoggedIn => AdminId.HasValue;
    }
}
