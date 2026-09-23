using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Model.Models.ViewModels;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<PagedResult<BloodRequest>> GetPagedRequestsAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var (items, totalCount) = await _requestRepository.GetPagedAsync(
                page,
                pageSize,
                predicate: null,
                orderBy: q => q.OrderByDescending(r => r.ApplyDate));

            return new PagedResult<BloodRequest>
            {
                Items = items,
                PageIndex = page,
                PageSize = pageSize,
                TotalItems = totalCount
            };
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
            var query = _requestRepository.Query();

            if (!string.IsNullOrWhiteSpace(bloodGroup) && bloodGroup != "All")
            {
                query = query.Where(r => r.BloodGroup == bloodGroup || r.BloodRequireFor == bloodGroup);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                var loc = location.Trim();
                query = query.Where(r =>
                    (r.Location != null && r.Location.Contains(loc)) ||
                    (r.HospitalName != null && r.HospitalName.Contains(loc)));
            }

            return await query.OrderByDescending(r => r.ApplyDate).ToListAsync();
        }
    }
}
