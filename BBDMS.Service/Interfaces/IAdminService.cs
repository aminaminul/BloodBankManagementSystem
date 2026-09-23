using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Service.Interfaces
{
    public interface IAdminService
    {
        Task<Admin> LoginAsync(string username, string password);
        Task<int> GetTotalDonorsCountAsync();
        Task<int> GetTotalRequestsCountAsync();
        Task<int> GetTotalHospitalsCountAsync();
        Task<int> GetTotalAvailableIcuBedsAsync();
        Task<int> GetTotalAmbulancesCountAsync();
        Task<int> GetTotalOxygenSuppliersCountAsync();
        Task<int> GetTotalBloodBanksCountAsync();
    }
}
