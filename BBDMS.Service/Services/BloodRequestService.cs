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
            var requests = await _requestRepository.GetAllAsync();
            return requests.OrderByDescending(r => r.ApplyDate).ToList();
        }

        public async Task<BloodRequest> GetRequestByIdAsync(int id)
        {
            return await _requestRepository.GetByIdAsync(id);
        }

        public async Task UpdateRequestStatusAsync(int id, string status)
        {
            var request = await _requestRepository.GetByIdAsync(id);
            if (request != null)
            {
                request.Status = status;
                _requestRepository.Update(request);
                await _requestRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<BloodRequest>> SearchEmergencyRequestsAsync(string? bloodGroup, string? location)
        {
            var requests = await _requestRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(bloodGroup) && bloodGroup != "All")
            {
                requests = requests.Where(r => r.BloodGroup == bloodGroup || r.BloodRequireFor == bloodGroup);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                var loc = location.Trim().ToLower();
                requests = requests.Where(r =>
                    (r.Location != null && r.Location.ToLower().Contains(loc)) ||
                    (r.HospitalName != null && r.HospitalName.ToLower().Contains(loc)));
            }

            return requests.OrderByDescending(r => r.ApplyDate).ToList();
        }
    }
}
