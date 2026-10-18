using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Model.Models.ViewModels;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;
using BBDMS.Web.Filters;
using Microsoft.AspNetCore.Http;

namespace BBDMS.Web.Services
{
    // Audit Trail Logging Service
    public class AuditService : IAuditService
    {
        private readonly IRepository<AuditLog> _auditRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(IRepository<AuditLog> auditRepository, IHttpContextAccessor httpContextAccessor)
        {
            _auditRepository = auditRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string action, string entityType, string? entityId = null, string? details = null)
        {
            try
            {
                var http = _httpContextAccessor.HttpContext;
                var user = http?.User;

                var log = new AuditLog
                {
                    Action = Truncate(action, 60) ?? string.Empty,
                    EntityType = Truncate(entityType, 60) ?? string.Empty,
                    EntityId = Truncate(entityId, 60),
                    Details = Truncate(details, 500),
                    ActorRole = user?.IsInRole(Roles.Admin) == true ? "Admin"
                              : user?.IsInRole(Roles.Donor) == true ? "Donor"
                              : "Anonymous",
                    ActorId = int.TryParse(user?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null,
                    ActorName = Truncate(user?.Identity?.Name, 120),
                    IpAddress = http?.Connection?.RemoteIpAddress?.ToString(),
                    CreatedAt = DateTime.Now
                };

                await _auditRepository.AddAsync(log);
                await _auditRepository.SaveChangesAsync();
            }
            catch (Exception)
            {
            }
        }

        public async Task<PagedResult<AuditLog>> GetPagedAsync(int page = 1, int pageSize = 25)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 25;

            var (items, totalCount) = await _auditRepository.GetPagedAsync(
                page,
                pageSize,
                predicate: null,
                orderBy: q => q.OrderByDescending(a => a.CreatedAt).ThenByDescending(a => a.Id));

            return new PagedResult<AuditLog>
            {
                Items = items,
                PageIndex = page,
                PageSize = pageSize,
                TotalItems = totalCount
            };
        }

        private static string? Truncate(string? value, int max)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= max ? value : value.Substring(0, max);
        }
    }
}
