using System.Collections.Generic;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Service.Interfaces
{
    public interface IBloodBankService
    {
        Task<IEnumerable<BloodBank>> GetAllBloodBanksAsync();
        Task<BloodBank> GetBloodBankByIdAsync(int id);
        Task<IEnumerable<BloodBank>> SearchBloodBanksAsync(string? location, string? bloodGroup);
        Task AddBloodBankAsync(BloodBank bloodBank);
        Task UpdateBloodBankAsync(BloodBank bloodBank);
        Task DeleteBloodBankAsync(int id);
    }
}
