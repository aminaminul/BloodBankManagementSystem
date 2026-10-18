using System;
using System.Collections.Generic;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Model.Models.ViewModels
{
    public class AdminReportsViewModel
    {
        public int TotalDonors { get; set; }
        public int TotalRequests { get; set; }
        public int TotalHospitals { get; set; }
        public int TotalBloodBanks { get; set; }
        public int TotalAmbulances { get; set; }
        public int TotalOxygen { get; set; }

        public int FulfilledRequests { get; set; }
        public int PendingRequests { get; set; }
        public int EmergencyRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int AvailableDonors { get; set; }

        // Donation history (real data)
        public int TotalDonations { get; set; }
        public int TotalUnitsDonated { get; set; }
        public IEnumerable<Donation> Donations { get; set; } = new List<Donation>();

        public IEnumerable<BloodDonor> Donors { get; set; } = new List<BloodDonor>();
        public IEnumerable<BloodRequest> Requests { get; set; } = new List<BloodRequest>();

        // Time series (last 12 months, straight from the database)
        public IReadOnlyList<string> SeriesLabels { get; set; } = new List<string>();
        public IReadOnlyList<int> UnitsPerMonth { get; set; } = new List<int>();
        public IReadOnlyList<int> DonationsPerMonth { get; set; } = new List<int>();
        public IReadOnlyList<int> RequestsPerMonth { get; set; } = new List<int>();
        public IReadOnlyList<int> NewDonorsPerMonth { get; set; } = new List<int>();
        public int DonationsLast30Days { get; set; }
        public int RequestsLast30Days { get; set; }
        public DateTime SeriesStart { get; set; }
    }
}
