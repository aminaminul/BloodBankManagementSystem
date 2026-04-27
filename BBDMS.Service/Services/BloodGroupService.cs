using System.Collections.Generic;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;

namespace BBDMS.Service.Services
{
    public class BloodGroupService : IBloodGroupService
    {
        private readonly IRepository<BloodGroup> _bloodGroupRepository;

        public BloodGroupService(IRepository<BloodGroup> bloodGroupRepository)
        {
            _bloodGroupRepository = bloodGroupRepository;
        }

        public async Task<IEnumerable<BloodGroup>> GetAllGroupsAsync()
        {
            return await _bloodGroupRepository.GetAllAsync();
        }
    }
}
