using System.Collections.Generic;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Model.Models.ViewModels;

namespace BBDMS.Service.Interfaces
{
    public interface IBloodRequestService
    {
        Task SaveRequestAsync(BloodRequest request);
        Task<IEnumerable<BloodRequest>> GetRequestsByDonorIdAsync(int donorId);
        Task<IEnumerable<BloodRequest>> GetAllRequestsAsync();
        Task<PagedResult<BloodRequest>> GetPagedRequestsAsync(int page, int pageSize);
        Task<BloodRequest> GetRequestByIdAsync(int id);
        Task UpdateRequestStatusAsync(int id, string status);
        Task<IEnumerable<BloodRequest>> SearchEmergencyRequestsAsync(string? bloodGroup, string? location);
        Task<IEnumerable<BloodRequest>> GetOpenCommunityRequestsAsync(string? bloodGroup, int take = 50);
        Task<BloodRequest?> GetLatestRequestByContactAsync(string contact);
        Task<BloodRequest?> GetRequestByTokenAsync(string token);
    }
}
