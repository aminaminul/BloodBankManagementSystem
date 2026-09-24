using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;

namespace BBDMS.Service.Services
{
    public class HospitalService : IHospitalService
    {
        private readonly IRepository<Hospital> _hospitalRepository;

        public HospitalService(IRepository<Hospital> hospitalRepository)
        {
            _hospitalRepository = hospitalRepository;
        }

        public async Task<IEnumerable<Hospital>> GetAllHospitalsAsync()
        {
            return await _hospitalRepository.GetAllAsync();
        }

        public async Task<Hospital> GetHospitalByIdAsync(int id)
        {
            return await _hospitalRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Hospital>> SearchHospitalsAsync(string location, bool? icuOnly, bool? emergencyBedOnly)
        {
            var query = _hospitalRepository.Query();

            if (!string.IsNullOrWhiteSpace(location))
            {
                var loc = location.Trim();
                query = query.Where(h =>
                    (h.City != null && h.City.Contains(loc)) ||
                    (h.Address != null && h.Address.Contains(loc)) ||
                    (h.Name != null && h.Name.Contains(loc)));
            }

            if (icuOnly == true)
            {
                query = query.Where(h => h.AvailableIcuBeds > 0);
            }

            if (emergencyBedOnly == true)
            {
                query = query.Where(h => h.AvailableEmergencyBeds > 0);
            }

            return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(query);
        }

        public async Task AddHospitalAsync(Hospital hospital)
        {
            hospital.LastUpdated = System.DateTime.Now;
            await _hospitalRepository.AddAsync(hospital);
            await _hospitalRepository.SaveChangesAsync();
        }

        public async Task UpdateHospitalAsync(Hospital hospital)
        {
            hospital.LastUpdated = System.DateTime.Now;
            _hospitalRepository.Update(hospital);
            await _hospitalRepository.SaveChangesAsync();
        }

        public async Task DeleteHospitalAsync(int id)
        {
            var hospital = await _hospitalRepository.GetByIdAsync(id);
            if (hospital != null)
            {
                _hospitalRepository.Remove(hospital);
                await _hospitalRepository.SaveChangesAsync();
            }
        }
    }
}
