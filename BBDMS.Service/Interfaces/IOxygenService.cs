using System.Collections.Generic;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Service.Interfaces
{
    public interface IOxygenService
    {
        Task<IEnumerable<OxygenService>> GetAllOxygenServicesAsync();
        Task<OxygenService> GetOxygenServiceByIdAsync(int id);
        Task<IEnumerable<OxygenService>> SearchOxygenServicesAsync(string location, bool? homeDeliveryOnly);
        Task AddOxygenServiceAsync(OxygenService oxygenService);
        Task UpdateOxygenServiceAsync(OxygenService oxygenService);
        Task DeleteOxygenServiceAsync(int id);
    }
}
