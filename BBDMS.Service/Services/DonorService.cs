using System.Collections.Generic;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;

namespace BBDMS.Service.Services
{
    public class DonorService : IDonorService
    {
        private readonly IRepository<BloodDonor> _donorRepository;

        public DonorService(IRepository<BloodDonor> donorRepository)
        {
            _donorRepository = donorRepository;
        }

        public async Task<IEnumerable<BloodDonor>> GetAllDonorsAsync()
        {
            return await _donorRepository.GetAllAsync();
        }

        public async Task<BloodDonor> GetDonorByIdAsync(int id)
        {
            return await _donorRepository.GetByIdAsync(id);
        }

        public async Task RegisterDonorAsync(BloodDonor donor)
        {
            await _donorRepository.AddAsync(donor);
            await _donorRepository.SaveChangesAsync();
        }

        public async Task UpdateDonorAsync(BloodDonor donor)
        {
            _donorRepository.Update(donor);
            await _donorRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<BloodDonor>> SearchDonorsAsync(string bloodGroup, string location)
        {
            return await _donorRepository.FindAsync(d => 
                (string.IsNullOrEmpty(bloodGroup) || d.BloodGroup == bloodGroup) &&
                (string.IsNullOrEmpty(location) || d.Address.Contains(location)) &&
                d.Status == 1);
        }
    }
}
