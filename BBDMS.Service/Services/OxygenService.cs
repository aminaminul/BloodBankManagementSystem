using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;

namespace BBDMS.Service.Services
{
    public class OxygenServiceImplementation : IOxygenService
    {
        private readonly IRepository<OxygenService> _oxygenRepository;

        public OxygenServiceImplementation(IRepository<OxygenService> oxygenRepository)
        {
            _oxygenRepository = oxygenRepository;
        }

        public async Task<IEnumerable<OxygenService>> GetAllOxygenServicesAsync()
        {
            return await _oxygenRepository.GetAllAsync();
        }

        public async Task<OxygenService> GetOxygenServiceByIdAsync(int id)
        {
            return await _oxygenRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<OxygenService>> SearchOxygenServicesAsync(string location, bool? homeDeliveryOnly)
        {
            var query = _oxygenRepository.Query();

            if (!string.IsNullOrWhiteSpace(location))
            {
                var loc = location.Trim();
                query = query.Where(s =>
                    (s.Location != null && s.Location.Contains(loc)) ||
                    (s.Address != null && s.Address.Contains(loc)) ||
                    (s.ProviderName != null && s.ProviderName.Contains(loc)));
            }

            if (homeDeliveryOnly == true)
            {
                query = query.Where(s => s.HomeDeliveryAvailable);
            }

            return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(query);
        }

        public async Task AddOxygenServiceAsync(OxygenService oxygenService)
        {
            oxygenService.LastUpdated = System.DateTime.Now;
            await _oxygenRepository.AddAsync(oxygenService);
            await _oxygenRepository.SaveChangesAsync();
        }

        public async Task UpdateOxygenServiceAsync(OxygenService oxygenService)
        {
            oxygenService.LastUpdated = System.DateTime.Now;
            _oxygenRepository.Update(oxygenService);
            await _oxygenRepository.SaveChangesAsync();
        }

        public async Task DeleteOxygenServiceAsync(int id)
        {
            var service = await _oxygenRepository.GetByIdAsync(id);
            if (service != null)
            {
                _oxygenRepository.Remove(service);
                await _oxygenRepository.SaveChangesAsync();
            }
        }
    }
}
