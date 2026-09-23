using System.Collections.Generic;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Model.Models.ViewModels
{
    public class EmergencySearchViewModel
    {
        public string Location { get; set; }
        public string BloodGroup { get; set; }
        public string ServiceType { get; set; } = "All"; // "All", "Donors", "Hospitals", "BloodBanks", "Ambulance", "Oxygen"

        public IEnumerable<BloodDonor> Donors { get; set; } = new List<BloodDonor>();
        public IEnumerable<Hospital> Hospitals { get; set; } = new List<Hospital>();
        public IEnumerable<BloodBank> BloodBanks { get; set; } = new List<BloodBank>();
        public IEnumerable<AmbulanceService> Ambulances { get; set; } = new List<AmbulanceService>();
        public IEnumerable<OxygenService> OxygenServices { get; set; } = new List<OxygenService>();
        public IEnumerable<BloodRequest> EmergencyRequests { get; set; } = new List<BloodRequest>();
    }
}
