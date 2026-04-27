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

        public AdminService(IRepository<Admin> adminRepository, IRepository<BloodDonor> donorRepository, IRepository<BloodRequest> requestRepository)
        {
            _adminRepository = adminRepository;
            _donorRepository = donorRepository;
            _requestRepository = requestRepository;
        }

        public async Task<Admin> LoginAsync(string username, string password)
        {
            var admins = await _adminRepository.GetAllAsync();
            return admins.FirstOrDefault(a => a.UserName == username && a.Password == password);
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
    }
}
