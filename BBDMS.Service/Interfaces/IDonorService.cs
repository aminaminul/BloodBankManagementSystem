using System.Collections.Generic;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Model.Models.ViewModels;

namespace BBDMS.Service.Interfaces
{
    public interface IDonorService
    {
        Task<IEnumerable<BloodDonor>> GetAllDonorsAsync();
        Task<PagedResult<BloodDonor>> GetPagedDonorsAsync(int page, int pageSize);
        Task<BloodDonor> GetDonorByIdAsync(int id);
        Task RegisterDonorAsync(BloodDonor donor);
        Task UpdateDonorAsync(BloodDonor donor);
        Task<IEnumerable<BloodDonor>> SearchDonorsAsync(string? bloodGroup, string? location);
        Task ToggleAvailabilityAsync(int id);
        Task ToggleStatusAsync(int id);
        Task DeleteDonorAsync(int id);
    }
}
