using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BBDMS.Service.Services
{
    public class AmbulanceHelpService : IAmbulanceService
    {
        private readonly IRepository<AmbulanceService> _ambulanceRepository;
        private readonly IRepository<AmbulanceRequest> _requestRepository;

        public AmbulanceHelpService(IRepository<AmbulanceService> ambulanceRepository, IRepository<AmbulanceRequest> requestRepository)
        {
            _ambulanceRepository = ambulanceRepository;
            _requestRepository = requestRepository;
        }

        public async Task<IEnumerable<AmbulanceService>> GetAllAmbulancesAsync()
        {
            return await _ambulanceRepository.GetAllAsync();
        }

        public async Task<AmbulanceService> GetAmbulanceByIdAsync(int id)
        {
            return await _ambulanceRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<AmbulanceService>> SearchAmbulancesAsync(string? location, string? ambulanceType, bool? onlyAvailable)
        {
            var query = _ambulanceRepository.Query();

            if (!string.IsNullOrWhiteSpace(location))
            {
                var loc = location.Trim();
                query = query.Where(a =>
                    (a.Location != null && a.Location.Contains(loc)) ||
                    (a.ProviderName != null && a.ProviderName.Contains(loc)));
            }

            if (!string.IsNullOrWhiteSpace(ambulanceType) && ambulanceType != "All")
            {
                var at = ambulanceType.Trim();
                query = query.Where(a => a.AmbulanceType != null && a.AmbulanceType.Contains(at));
            }

            if (onlyAvailable == true)
            {
                query = query.Where(a => a.IsAvailable);
            }

            return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(query);
        }

        public async Task AddAmbulanceAsync(AmbulanceService ambulance)
        {
            ambulance.LastUpdated = System.DateTime.Now;
            await _ambulanceRepository.AddAsync(ambulance);
            await _ambulanceRepository.SaveChangesAsync();
        }

        public async Task UpdateAmbulanceAsync(AmbulanceService ambulance)
        {
            ambulance.LastUpdated = System.DateTime.Now;
            _ambulanceRepository.Update(ambulance);
            await _ambulanceRepository.SaveChangesAsync();
        }

        public async Task DeleteAmbulanceAsync(int id)
        {
            var ambulance = await _ambulanceRepository.GetByIdAsync(id);
            if (ambulance != null)
            {
                _ambulanceRepository.Remove(ambulance);
                await _ambulanceRepository.SaveChangesAsync();
            }
        }

        public async Task SubmitAmbulanceRequestAsync(AmbulanceRequest request)
        {
            request.RequestDate = System.DateTime.Now;
            if (string.IsNullOrEmpty(request.Status))
            {
                request.Status = "Pending";
            }

            if (string.IsNullOrWhiteSpace(request.TrackingToken))
            {
                request.TrackingToken = NewTrackingToken();
            }

            await _requestRepository.AddAsync(request);
            await _requestRepository.SaveChangesAsync();
        }

        // Generate Tracking Token
        public static string NewTrackingToken() => System.Guid.NewGuid().ToString("N");

        public async Task<AmbulanceRequest?> GetAmbulanceRequestByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            var normalized = token.Trim();
            return await _requestRepository.Query()
                .Where(r => r.TrackingToken == normalized)
                .FirstOrDefaultAsync();
        }

        public async Task<AmbulanceRequest?> GetLatestAmbulanceRequestByContactAsync(string contact)
        {
            if (string.IsNullOrWhiteSpace(contact)) return null;

            var digits = new string(contact.Where(char.IsDigit).ToArray());
            if (digits.Length == 0) return null;

            return await _requestRepository.Query()
                .Where(r => r.ContactNumber != null && r.ContactNumber.Contains(digits))
                .OrderByDescending(r => r.RequestDate)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AmbulanceRequest>> GetAllAmbulanceRequestsAsync()
        {
            var requests = await _requestRepository.GetAllAsync();
            return requests.OrderByDescending(r => r.RequestDate).ToList();
        }

        public async Task<AmbulanceRequest> GetAmbulanceRequestByIdAsync(int id)
        {
            return await _requestRepository.GetByIdAsync(id);
        }

        public async Task UpdateAmbulanceRequestStatusAsync(int id, string status)
        {
            var request = await _requestRepository.GetByIdAsync(id);
            if (request != null)
            {
                request.Status = status;
                _requestRepository.Update(request);
                await _requestRepository.SaveChangesAsync();
            }
        }
    }
}
