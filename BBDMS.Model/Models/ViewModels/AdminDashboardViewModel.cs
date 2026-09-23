namespace BBDMS.Model.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalDonors { get; set; }
        public int TotalRequests { get; set; }
        public int TotalHospitals { get; set; }
        public int TotalAvailableIcuBeds { get; set; }
        public int TotalAmbulances { get; set; }
        public int TotalOxygenSuppliers { get; set; }
        public int TotalBloodBanks { get; set; }
    }
}
