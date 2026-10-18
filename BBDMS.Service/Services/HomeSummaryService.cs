using System.Linq;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Model.Models.ViewModels;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BBDMS.Service.Services
{
    // Home Landing Summary Service
    public class HomeSummaryService : IHomeSummaryService
    {
        private readonly IRepository<BloodDonor> _donorRepository;
        private readonly IRepository<Hospital> _hospitalRepository;
        private readonly IRepository<BloodBank> _bloodBankRepository;
        private readonly IRepository<AmbulanceService> _ambulanceRepository;
        private readonly IRepository<OxygenService> _oxygenRepository;
        private readonly IRepository<BloodRequest> _requestRepository;

        public HomeSummaryService(
            IRepository<BloodDonor> donorRepository,
            IRepository<Hospital> hospitalRepository,
            IRepository<BloodBank> bloodBankRepository,
            IRepository<AmbulanceService> ambulanceRepository,
            IRepository<OxygenService> oxygenRepository,
            IRepository<BloodRequest> requestRepository)
        {
            _donorRepository = donorRepository;
            _hospitalRepository = hospitalRepository;
            _bloodBankRepository = bloodBankRepository;
            _ambulanceRepository = ambulanceRepository;
            _oxygenRepository = oxygenRepository;
            _requestRepository = requestRepository;
        }

        public async Task<HomeIndexViewModel> GetSummaryAsync()
        {
            return new HomeIndexViewModel
            {
                TotalDonors = await _donorRepository.CountAsync(),
                TotalHospitals = await _hospitalRepository.CountAsync(),
                TotalAvailableIcuBeds = await _hospitalRepository.SumAsync(h => h.AvailableIcuBeds),
                TotalAvailableEmergencyBeds = await _hospitalRepository.SumAsync(h => h.AvailableEmergencyBeds),
                TotalBloodBanks = await _bloodBankRepository.CountAsync(),
                TotalAmbulances = await _ambulanceRepository.CountAsync(),
                TotalOxygenSuppliers = await _oxygenRepository.CountAsync(),
                TotalActiveRequests = await _requestRepository.CountAsync(
                    r => r.Status != "Fulfilled" && r.Status != "Cancelled" &&
                         r.Status != "Completed" && r.Status != "Rejected"),

                ActiveDonors = await _donorRepository.Query()
                    .Where(d => d.Status == 1)
                    .OrderByDescending(d => d.Id)
                    .Take(6)
                    .ToListAsync(),

                UrgentRequests = await _requestRepository.Query()
                    .Where(r => r.Status != "Fulfilled" && r.Status != "Cancelled" &&
                                r.Status != "Completed" && r.Status != "Rejected")
                    .OrderByDescending(r => r.ID)
                    .Take(3)
                    .ToListAsync(),

                FeaturedHospitals = await _hospitalRepository.Query()
                    .OrderBy(h => h.Id)
                    .Take(3)
                    .ToListAsync(),

                FeaturedBloodBanks = await _bloodBankRepository.Query()
                    .OrderBy(b => b.Id)
                    .Take(3)
                    .ToListAsync(),

                FeaturedAmbulances = await _ambulanceRepository.Query()
                    .Where(a => a.IsAvailable)
                    .OrderBy(a => a.Id)
                    .Take(3)
                    .ToListAsync(),

                FeaturedOxygenSuppliers = await _oxygenRepository.Query()
                    .OrderBy(o => o.Id)
                    .Take(3)
                    .ToListAsync()
            };
        }
    }
}
