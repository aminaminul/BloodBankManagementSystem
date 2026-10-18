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
        Task<BloodDonor?> GetDonorByEmailAsync(string email);
        Task RegisterDonorAsync(BloodDonor donor);
        Task UpdateDonorAsync(BloodDonor donor);
        Task<PagedResult<DonorListItemViewModel>> SearchDonorsAsync(
            string? bloodGroup, string? location, int page = 1, int pageSize = 12, bool includeContact = false);
        Task<int> CountDonorsByBloodGroupAsync(string? bloodGroup, int excludeDonorId = 0);
        Task ToggleAvailabilityAsync(int id);
        Task ToggleStatusAsync(int id);
        Task DeleteDonorAsync(int id);
    }
}
