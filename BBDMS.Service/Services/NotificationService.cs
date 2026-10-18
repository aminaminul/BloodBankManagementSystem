using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BBDMS.Service.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification> _notificationRepo;

        public NotificationService(IRepository<Notification> notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            if (notification.CreatedAt == default)
            {
                notification.CreatedAt = DateTime.UtcNow;
            }

            await _notificationRepo.AddAsync(notification);
            await _notificationRepo.SaveChangesAsync();
            return notification;
        }

        public async Task<IEnumerable<Notification>> GetRecentNotificationsAsync(int? userId, string role, int limit = 10)
        {
            var query = _notificationRepo.Query();

            if (userId.HasValue && userId.Value > 0)
            {
                query = query.Where(n => 
                    (n.UserId == userId.Value || n.UserId == null || n.UserId == 0) &&
                    (n.TargetRole == "All" || n.TargetRole == role));
            }
            else
            {
                query = query.Where(n => 
                    (n.UserId == null || n.UserId == 0) &&
                    (n.TargetRole == "All" || n.TargetRole == role));
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int? userId, string role)
        {
            var query = _notificationRepo.Query().Where(n => !n.IsRead);

            if (userId.HasValue && userId.Value > 0)
            {
                query = query.Where(n => 
                    (n.UserId == userId.Value || n.UserId == null || n.UserId == 0) &&
                    (n.TargetRole == "All" || n.TargetRole == role));
            }
            else
            {
                query = query.Where(n => 
                    (n.UserId == null || n.UserId == 0) &&
                    (n.TargetRole == "All" || n.TargetRole == role));
            }

            return await query.CountAsync();
        }

        public async Task MarkAsReadAsync(int id)
        {
            var notification = await _notificationRepo.GetByIdAsync(id);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                _notificationRepo.Update(notification);
                await _notificationRepo.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(int? userId, string role)
        {
            var notifications = await GetRecentNotificationsAsync(userId, role, 100);
            foreach (var n in notifications.Where(x => !x.IsRead))
            {
                var tracked = await _notificationRepo.GetByIdAsync(n.Id);
                if (tracked != null)
                {
                    tracked.IsRead = true;
                    _notificationRepo.Update(tracked);
                }
            }
            await _notificationRepo.SaveChangesAsync();
        }
    }
}
