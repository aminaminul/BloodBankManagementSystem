using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Service.Interfaces
{
    public interface IAdminService
    {
        Task<Admin> LoginAsync(string username, string password);
        Task<int> GetTotalDonorsCountAsync();
        Task<int> GetTotalRequestsCountAsync();
    }
}
