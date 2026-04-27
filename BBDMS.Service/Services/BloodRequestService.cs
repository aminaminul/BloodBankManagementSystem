using System.Collections.Generic;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;

namespace BBDMS.Service.Services
{
    public class BloodRequestService : IBloodRequestService
    {
        private readonly IRepository<BloodRequest> _requestRepository;

        public BloodRequestService(IRepository<BloodRequest> requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task SaveRequestAsync(BloodRequest request)
        {
            await _requestRepository.AddAsync(request);
            await _requestRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<BloodRequest>> GetRequestsByDonorIdAsync(int donorId)
        {
            return await _requestRepository.FindAsync(r => r.BloodDonorID == donorId);
        }

        public async Task<IEnumerable<BloodRequest>> GetAllRequestsAsync()
        {
            return await _requestRepository.GetAllAsync();
        }
    }
}
