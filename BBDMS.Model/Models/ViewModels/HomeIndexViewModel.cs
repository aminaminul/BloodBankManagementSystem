using System.Collections.Generic;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Model.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        // Live counts directly from Database
        public int TotalDonors { get; set; }
        public int TotalHospitals { get; set; }
        public int TotalAvailableIcuBeds { get; set; }
        public int TotalAvailableEmergencyBeds { get; set; }
        public int TotalBloodBanks { get; set; }
        public int TotalAmbulances { get; set; }
        public int TotalOxygenSuppliers { get; set; }
        public int TotalActiveRequests { get; set; }

        // Live Data Collections from Database
        public ContactInfo? ContactInfo { get; set; }
        public IEnumerable<BloodGroup> BloodGroups { get; set; } = new List<BloodGroup>();
        public IEnumerable<BloodDonor> ActiveDonors { get; set; } = new List<BloodDonor>();
        public IEnumerable<BloodRequest> UrgentRequests { get; set; } = new List<BloodRequest>();
        public IEnumerable<Hospital> FeaturedHospitals { get; set; } = new List<Hospital>();
        public IEnumerable<BloodBank> FeaturedBloodBanks { get; set; } = new List<BloodBank>();
        public IEnumerable<AmbulanceService> FeaturedAmbulances { get; set; } = new List<AmbulanceService>();
        public IEnumerable<OxygenService> FeaturedOxygenSuppliers { get; set; } = new List<OxygenService>();
    }
}
