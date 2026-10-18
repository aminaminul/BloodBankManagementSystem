using System;

namespace BBDMS.Model.Models.ViewModels
{
    /// <summary>
    /// Public-facing donor card. Deliberately contains no password hash, and the contact fields are
    /// masked unless <see cref="ContactVisible"/> is true, because the donor directory is browsable
    /// by anonymous visitors.
    /// </summary>
    public class DonorListItemViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public int? Age { get; set; }
        public string? Address { get; set; }
        public string? Message { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? LastDonatedDate { get; set; }

        /// <summary>Masked ("Sign in to view") unless the viewer is allowed to see contact details.</summary>
        public string MobileNumber { get; set; } = string.Empty;

        /// <summary>Masked or empty unless the viewer is allowed to see contact details.</summary>
        public string EmailId { get; set; } = string.Empty;

        public bool ContactVisible { get; set; }

        private const int EligibilityGapDays = 90;

        public bool IsEligibleToDonate =>
            !LastDonatedDate.HasValue || (DateTime.Now - LastDonatedDate.Value).TotalDays >= EligibilityGapDays;

        public int DaysUntilEligible =>
            !LastDonatedDate.HasValue
                ? 0
                : Math.Max(0, EligibilityGapDays - (int)(DateTime.Now - LastDonatedDate.Value).TotalDays);
    }
}
