using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;

namespace BBDMS.Service.Services
{
    public class BloodBankService : IBloodBankService
    {
        private readonly IRepository<BloodBank> _bloodBankRepository;

        public BloodBankService(IRepository<BloodBank> bloodBankRepository)
        {
            _bloodBankRepository = bloodBankRepository;
        }

        public async Task<IEnumerable<BloodBank>> GetAllBloodBanksAsync()
        {
            return await _bloodBankRepository.GetAllAsync();
        }

        public async Task<BloodBank> GetBloodBankByIdAsync(int id)
        {
            return await _bloodBankRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<BloodBank>> SearchBloodBanksAsync(string? location, string? bloodGroup)
        {
            var query = _bloodBankRepository.Query();

            if (!string.IsNullOrWhiteSpace(location))
            {
                var loc = location.Trim();
                query = query.Where(b =>
                    (b.City != null && b.City.Contains(loc)) ||
                    (b.Address != null && b.Address.Contains(loc)) ||
                    (b.Name != null && b.Name.Contains(loc)));
            }

            if (!string.IsNullOrWhiteSpace(bloodGroup))
            {
                var bg = bloodGroup.Trim();
                query = query.Where(b => b.AvailableBloodGroups != null && b.AvailableBloodGroups.Contains(bg));
            }

            return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(query);
        }

        public async Task AddBloodBankAsync(BloodBank bloodBank)
        {
            bloodBank.LastUpdated = System.DateTime.Now;
            await _bloodBankRepository.AddAsync(bloodBank);
            await _bloodBankRepository.SaveChangesAsync();
        }

        public async Task UpdateBloodBankAsync(BloodBank bloodBank)
        {
            bloodBank.LastUpdated = System.DateTime.Now;
            _bloodBankRepository.Update(bloodBank);
            await _bloodBankRepository.SaveChangesAsync();
        }

        public async Task DeleteBloodBankAsync(int id)
        {
            var bank = await _bloodBankRepository.GetByIdAsync(id);
            if (bank != null)
            {
                _bloodBankRepository.Remove(bank);
                await _bloodBankRepository.SaveChangesAsync();
            }
        }
    }
}
