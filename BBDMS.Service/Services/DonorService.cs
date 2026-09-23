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

        public async Task<PagedResult<BloodDonor>> GetPagedDonorsAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var (items, totalCount) = await _donorRepository.GetPagedAsync(
                page,
                pageSize,
                predicate: null,
                orderBy: q => q.OrderByDescending(d => d.PostingDate));

            return new PagedResult<BloodDonor>
            {
                Items = items,
                PageIndex = page,
                PageSize = pageSize,
                TotalItems = totalCount
            };
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

        public async Task<IEnumerable<BloodDonor>> SearchDonorsAsync(string? bloodGroup, string? location)
        {
            var query = _donorRepository.Query().Where(d => d.Status == 1);

            if (!string.IsNullOrEmpty(bloodGroup) && bloodGroup != "All")
            {
                query = query.Where(d => d.BloodGroup == bloodGroup);
            }

            if (!string.IsNullOrEmpty(location))
            {
                var loc = location.Trim();
                query = query.Where(d => d.Address != null && d.Address.Contains(loc));
            }

            return await query.ToListAsync();
        }

        public async Task ToggleAvailabilityAsync(int id)
        {
            var donor = await _donorRepository.GetByIdAsync(id);
            if (donor != null)
            {
                donor.IsAvailable = !donor.IsAvailable;
                _donorRepository.Update(donor);
                await _donorRepository.SaveChangesAsync();
            }
        }

        public async Task ToggleStatusAsync(int id)
        {
            var donor = await _donorRepository.GetByIdAsync(id);
            if (donor != null)
            {
                donor.Status = donor.Status == 1 ? 0 : 1;
                _donorRepository.Update(donor);
                await _donorRepository.SaveChangesAsync();
            }
        }

        public async Task DeleteDonorAsync(int id)
        {
            var donor = await _donorRepository.GetByIdAsync(id);
            if (donor != null)
            {
                _donorRepository.Remove(donor);
                await _donorRepository.SaveChangesAsync();
            }
        }
    }
}
