using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;

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
            var ambulances = await _ambulanceRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(location))
            {
                var loc = location.Trim().ToLower();
                ambulances = ambulances.Where(a =>
                    (a.Location != null && a.Location.ToLower().Contains(loc)) ||
                    (a.ProviderName != null && a.ProviderName.ToLower().Contains(loc)));
            }

            if (!string.IsNullOrWhiteSpace(ambulanceType) && ambulanceType != "All")
            {
                var at = ambulanceType.Trim().ToLower();
                ambulances = ambulances.Where(a => a.AmbulanceType != null && a.AmbulanceType.ToLower().Contains(at));
            }

            if (onlyAvailable == true)
            {
                ambulances = ambulances.Where(a => a.IsAvailable);
            }

            return ambulances.ToList();
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
            await _requestRepository.AddAsync(request);
            await _requestRepository.SaveChangesAsync();
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
