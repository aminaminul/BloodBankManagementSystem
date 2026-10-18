using System.Collections.Generic;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Service.Interfaces
{
    public interface IDonationService
    {
        Task<IEnumerable<Donation>> GetAllDonationsAsync();
        Task<IEnumerable<Donation>> GetDonationsByDonorAsync(int donorId);
        Task<Donation> GetDonationByIdAsync(int id);
        Task AddDonationAsync(Donation donation);
        Task DeleteDonationAsync(int id);
        Task<int> GetTotalDonationsCountAsync();
        Task<int> GetTotalUnitsDonatedAsync();
    }
}
