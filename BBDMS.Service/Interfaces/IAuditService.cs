using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Model.Models.ViewModels;

namespace BBDMS.Service.Interfaces
{
    // Audit Trail Service Interface
    public interface IAuditService
    {
        Task LogAsync(string action, string entityType, string? entityId = null, string? details = null);
        Task<PagedResult<AuditLog>> GetPagedAsync(int page = 1, int pageSize = 25);
    }
}
