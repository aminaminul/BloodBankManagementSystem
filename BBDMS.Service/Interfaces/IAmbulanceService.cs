using System.Collections.Generic;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Service.Interfaces
{
    public interface IAmbulanceService
    {
        Task<IEnumerable<AmbulanceService>> GetAllAmbulancesAsync();
        Task<AmbulanceService> GetAmbulanceByIdAsync(int id);
        Task<IEnumerable<AmbulanceService>> SearchAmbulancesAsync(string? location, string? ambulanceType, bool? onlyAvailable);
        Task AddAmbulanceAsync(AmbulanceService ambulance);
        Task UpdateAmbulanceAsync(AmbulanceService ambulance);
        Task DeleteAmbulanceAsync(int id);

        Task SubmitAmbulanceRequestAsync(AmbulanceRequest request);
        Task<IEnumerable<AmbulanceRequest>> GetAllAmbulanceRequestsAsync();
        Task<AmbulanceRequest> GetAmbulanceRequestByIdAsync(int id);
        Task UpdateAmbulanceRequestStatusAsync(int id, string status);
    }
}
