using System.Collections.Generic;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Service.Interfaces
{
    public interface IBloodGroupService
    {
        Task<IEnumerable<BloodGroup>> GetAllGroupsAsync();
    }
}
