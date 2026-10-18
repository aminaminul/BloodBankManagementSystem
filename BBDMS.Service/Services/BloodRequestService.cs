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
            if (string.IsNullOrWhiteSpace(request.TrackingToken))
            {
                request.TrackingToken = System.Guid.NewGuid().ToString("N");
            }
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

        // Search Emergency Requests
        public async Task<IEnumerable<BloodRequest>> SearchEmergencyRequestsAsync(string? bloodGroup, string? location)
        {
            var query = _requestRepository.Query();

            if (!string.IsNullOrWhiteSpace(bloodGroup) && bloodGroup != "All")
            {
                query = query.Where(r => r.BloodGroup == bloodGroup);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                var loc = location.Trim();
                query = query.Where(r =>
                    (r.Location != null && r.Location.Contains(loc)) ||
                    (r.HospitalName != null && r.HospitalName.Contains(loc)));
            }

            query = query.Where(r => r.Status != "Fulfilled" && r.Status != "Completed"
                                  && r.Status != "Cancelled" && r.Status != "Rejected");

            return await query.OrderByDescending(r => r.ApplyDate).ToListAsync();
        }

        // Open Community Requests
        public async Task<IEnumerable<BloodRequest>> GetOpenCommunityRequestsAsync(string? bloodGroup, int take = 50)
        {
            if (take < 1) take = 50;

            var query = _requestRepository.Query().Where(r => r.BloodDonorID == null);

            if (!string.IsNullOrWhiteSpace(bloodGroup))
            {
                var group = bloodGroup.Trim();
                query = query.Where(r => r.BloodGroup == group);
            }

            return await query
                .Where(r => r.Status != "Fulfilled" && r.Status != "Cancelled" && r.Status != "Rejected")
                .OrderByDescending(r => r.ApplyDate)
                .Take(take)
                .ToListAsync();
        }

        public async Task<BloodRequest?> GetLatestRequestByContactAsync(string contact)
        {
            if (string.IsNullOrWhiteSpace(contact)) return null;

            var digits = new string(contact.Where(char.IsDigit).ToArray());
            if (digits.Length == 0) return null;

            return await _requestRepository.Query()
                .Where(r => r.ContactNumber != null && r.ContactNumber.Contains(digits))
                .OrderByDescending(r => r.ID)
                .FirstOrDefaultAsync();
        }

        public async Task<BloodRequest?> GetRequestByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            var normalized = token.Trim();
            return await _requestRepository.Query()
                .Where(r => r.TrackingToken == normalized)
                .FirstOrDefaultAsync();
        }
    }
}
