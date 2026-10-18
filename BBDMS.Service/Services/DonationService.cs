using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BBDMS.Service.Services
{
    public class DonationService : IDonationService
    {
        private readonly IRepository<Donation> _donationRepository;
        private readonly IDonorService _donorService;

        public DonationService(IRepository<Donation> donationRepository, IDonorService donorService)
        {
            _donationRepository = donationRepository;
            _donorService = donorService;
        }

        public async Task<IEnumerable<Donation>> GetAllDonationsAsync()
        {
            return await _donationRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Donation>> GetDonationsByDonorAsync(int donorId)
        {
            var donations = await _donationRepository.FindAsync(d => d.DonorId == donorId);
            return donations.OrderByDescending(d => d.DonationDate).ToList();
        }

        public async Task<Donation> GetDonationByIdAsync(int id)
        {
            return await _donationRepository.GetByIdAsync(id);
        }

        public async Task AddDonationAsync(Donation donation)
        {
            await _donationRepository.AddAsync(donation);
            await _donationRepository.SaveChangesAsync();
            await SyncDonorEligibilityAsync(donation.DonorId);
        }

        public async Task DeleteDonationAsync(int id)
        {
            var donation = await _donationRepository.GetByIdAsync(id);
            if (donation == null) return;

            _donationRepository.Remove(donation);
            await _donationRepository.SaveChangesAsync();
            await SyncDonorEligibilityAsync(donation.DonorId);
        }

        public Task<int> GetTotalDonationsCountAsync()
        {
            return _donationRepository.CountAsync();
        }

        public Task<int> GetTotalUnitsDonatedAsync()
        {
            return _donationRepository.SumAsync(d => d.UnitsDonated);
        }

        // Synchronize Donor Eligibility Date
        private async Task SyncDonorEligibilityAsync(int donorId)
        {
            if (donorId <= 0) return;

            var donor = await _donorService.GetDonorByIdAsync(donorId);
            if (donor == null) return;

            donor.LastDonatedDate = await _donationRepository.Query()
                .Where(d => d.DonorId == donorId)
                .MaxAsync(d => (System.DateTime?)d.DonationDate);
            await _donorService.UpdateDonorAsync(donor);
        }
    }
}
