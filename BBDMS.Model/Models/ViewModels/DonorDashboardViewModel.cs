using System;
using System.Collections.Generic;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Model.Models.ViewModels
{
    public class DonorDashboardViewModel
    {
        public BloodDonor? Donor { get; set; }
        public IEnumerable<BloodRequest> DirectRequests { get; set; } = new List<BloodRequest>();
        public IEnumerable<BloodRequest> CommunityRequests { get; set; } = new List<BloodRequest>();
        public int NearbyDonors { get; set; }
        public int PendingRequests { get; set; }
        public int FulfilledRequests { get; set; }

        // Donation history (real data)
        public IEnumerable<Donation> Donations { get; set; } = new List<Donation>();
        public int TotalDonations { get; set; }
        public int TotalUnitsDonated { get; set; }
        public DateTime? LastDonationDate { get; set; }
    }
}
