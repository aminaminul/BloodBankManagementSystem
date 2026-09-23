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
            var hospitals = await _hospitalRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(location))
            {
                var loc = location.Trim().ToLower();
                hospitals = hospitals.Where(h =>
                    (h.City != null && h.City.ToLower().Contains(loc)) ||
                    (h.Address != null && h.Address.ToLower().Contains(loc)) ||
                    (h.Name != null && h.Name.ToLower().Contains(loc)));
            }

            if (icuOnly == true)
            {
                hospitals = hospitals.Where(h => h.AvailableIcuBeds > 0);
            }

            if (emergencyBedOnly == true)
            {
                hospitals = hospitals.Where(h => h.AvailableEmergencyBeds > 0);
            }

            return hospitals.ToList();
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
