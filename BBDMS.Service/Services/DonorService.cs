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

        // Get Donor By Email
        public async Task<BloodDonor?> GetDonorByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            var normalized = email.Trim().ToLowerInvariant();
            return await _donorRepository.Query()
                .Where(d => d.EmailId != null && d.EmailId.ToLower() == normalized)
                .OrderBy(d => d.Id)
                .FirstOrDefaultAsync();
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

        // Search Donors (Paged DTO)
        public async Task<PagedResult<DonorListItemViewModel>> SearchDonorsAsync(
            string? bloodGroup,
            string? location,
            int page = 1,
            int pageSize = 12,
            bool includeContact = false)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 60) pageSize = 12;

            var query = _donorRepository.Query().Where(d => d.Status == 1);

            if (!string.IsNullOrEmpty(bloodGroup) && bloodGroup != "All")
            {
                var group = bloodGroup;
                query = query.Where(d => d.BloodGroup == group);
            }

            if (!string.IsNullOrEmpty(location))
            {
                var loc = location.Trim();
                query = query.Where(d => d.Address != null && d.Address.Contains(loc));
            }

            var totalCount = await query.CountAsync();

            var rows = await query
                .OrderByDescending(d => d.IsAvailable)
                .ThenByDescending(d => d.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<DonorListItemViewModel>
            {
                Items = rows.Select(d => MapToListItem(d, includeContact)).ToList(),
                PageIndex = page,
                PageSize = pageSize,
                TotalItems = totalCount
            };
        }

        // DTO Mapping
        private static DonorListItemViewModel MapToListItem(BloodDonor donor, bool includeContact) => new()
        {
            Id = donor.Id,
            FullName = donor.FullName ?? string.Empty,
            BloodGroup = donor.BloodGroup ?? string.Empty,
            Gender = donor.Gender,
            Age = donor.Age,
            Address = donor.Address,
            Message = donor.Message,
            IsAvailable = donor.IsAvailable,
            LastDonatedDate = donor.LastDonatedDate,
            ContactVisible = includeContact,
            MobileNumber = includeContact ? (donor.MobileNumber ?? string.Empty) : "Sign in to view",
            EmailId = includeContact ? (donor.EmailId ?? string.Empty) : string.Empty
        };

        // Count Donors by Blood Group
        public async Task<int> CountDonorsByBloodGroupAsync(string? bloodGroup, int excludeDonorId = 0)
        {
            var query = _donorRepository.Query().Where(d => d.Status == 1 && d.Id != excludeDonorId);

            if (!string.IsNullOrWhiteSpace(bloodGroup))
            {
                var group = bloodGroup.Trim();
                query = query.Where(d => d.BloodGroup == group);
            }

            return await query.CountAsync();
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
