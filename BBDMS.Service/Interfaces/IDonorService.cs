using System.Collections.Generic;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Service.Interfaces
{
    public interface IDonorService
    {
        Task<IEnumerable<BloodDonor>> GetAllDonorsAsync();
        Task<BloodDonor> GetDonorByIdAsync(int id);
        Task RegisterDonorAsync(BloodDonor donor);
        Task UpdateDonorAsync(BloodDonor donor);
        Task<IEnumerable<BloodDonor>> SearchDonorsAsync(string bloodGroup, string location);
    }
}
