using System.Collections.Generic;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Service.Interfaces
{
    public interface IBloodRequestService
    {
        Task SaveRequestAsync(BloodRequest request);
        Task<IEnumerable<BloodRequest>> GetRequestsByDonorIdAsync(int donorId);
        Task<IEnumerable<BloodRequest>> GetAllRequestsAsync();
    }
}
