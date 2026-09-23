using System.Collections.Generic;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;
using System.Linq;

namespace BBDMS.Service.Services
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<Admin> _adminRepository;
        private readonly IRepository<BloodDonor> _donorRepository;
        private readonly IRepository<BloodRequest> _requestRepository;
        private readonly IRepository<Hospital> _hospitalRepository;
        private readonly IRepository<AmbulanceService> _ambulanceRepository;
        private readonly IRepository<OxygenService> _oxygenRepository;
        private readonly IRepository<BloodBank> _bloodBankRepository;

        public AdminService(
            IRepository<Admin> adminRepository,
            IRepository<BloodDonor> donorRepository,
            IRepository<BloodRequest> requestRepository,
            IRepository<Hospital> hospitalRepository,
            IRepository<AmbulanceService> ambulanceRepository,
            IRepository<OxygenService> oxygenRepository,
            IRepository<BloodBank> bloodBankRepository)
        {
            _adminRepository = adminRepository;
            _donorRepository = donorRepository;
            _requestRepository = requestRepository;
            _hospitalRepository = hospitalRepository;
            _ambulanceRepository = ambulanceRepository;
            _oxygenRepository = oxygenRepository;
            _bloodBankRepository = bloodBankRepository;
        }

        public async Task<Admin> LoginAsync(string username, string password)
        {
            var admins = await _adminRepository.GetAllAsync();
            return admins.FirstOrDefault(a => a.UserName == username && a.Password == password)!;
        }

        public async Task<int> GetTotalDonorsCountAsync()
        {
            var donors = await _donorRepository.GetAllAsync();
            return donors.Count();
        }

        public async Task<int> GetTotalRequestsCountAsync()
        {
            var requests = await _requestRepository.GetAllAsync();
            return requests.Count();
        }

        public async Task<int> GetTotalHospitalsCountAsync()
        {
            var hospitals = await _hospitalRepository.GetAllAsync();
            return hospitals.Count();
        }

        public async Task<int> GetTotalAvailableIcuBedsAsync()
        {
            var hospitals = await _hospitalRepository.GetAllAsync();
            return hospitals.Sum(h => h.AvailableIcuBeds);
        }

        public async Task<int> GetTotalAmbulancesCountAsync()
        {
            var ambulances = await _ambulanceRepository.GetAllAsync();
            return ambulances.Count();
        }

        public async Task<int> GetTotalOxygenSuppliersCountAsync()
        {
            var oxygen = await _oxygenRepository.GetAllAsync();
            return oxygen.Count();
        }

        public async Task<int> GetTotalBloodBanksCountAsync()
        {
            var banks = await _bloodBankRepository.GetAllAsync();
            return banks.Count();
        }
    }
}
