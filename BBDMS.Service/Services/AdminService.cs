using System.Collections.Generic;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;
using BBDMS.Service.Common;
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
            var admin = admins.FirstOrDefault(a => a.UserName == username);
            if (admin == null) return null!;

            if (PasswordHasher.VerifyPassword(password, admin.Password, out bool needsRehash))
            {
                if (needsRehash)
                {
                    admin.Password = PasswordHasher.HashPassword(password);
                    _adminRepository.Update(admin);
                    await _adminRepository.SaveChangesAsync();
                }
                return admin;
            }

            return null!;
        }

        public async Task<int> GetTotalDonorsCountAsync()
        {
            return await _donorRepository.CountAsync();
        }

        public async Task<int> GetTotalRequestsCountAsync()
        {
            return await _requestRepository.CountAsync();
        }

        public async Task<int> GetTotalHospitalsCountAsync()
        {
            return await _hospitalRepository.CountAsync();
        }

        public async Task<int> GetTotalAvailableIcuBedsAsync()
        {
            return await _hospitalRepository.SumAsync(h => h.AvailableIcuBeds);
        }

        public async Task<int> GetTotalAmbulancesCountAsync()
        {
            return await _ambulanceRepository.CountAsync();
        }

        public async Task<int> GetTotalOxygenSuppliersCountAsync()
        {
            return await _oxygenRepository.CountAsync();
        }

        public async Task<int> GetTotalBloodBanksCountAsync()
        {
            return await _bloodBankRepository.CountAsync();
        }
    }
}
