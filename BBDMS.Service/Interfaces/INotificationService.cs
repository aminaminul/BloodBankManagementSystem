using System.Collections.Generic;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Service.Interfaces
{
    public interface INotificationService
    {
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task<IEnumerable<Notification>> GetRecentNotificationsAsync(int? userId, string role, int limit = 10);
        Task<int> GetUnreadCountAsync(int? userId, string role);
        Task MarkAsReadAsync(int id);
        Task MarkAllAsReadAsync(int? userId, string role);
    }
}
