using System;
using System.Linq;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Service.Interfaces;
using BBDMS.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BBDMS.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent(int limit = 10)
        {
            var userId = User.GetUserId();
            var role = User.IsInRole(Roles.Admin) ? Roles.Admin :
                       User.IsInRole(Roles.Donor) ? Roles.Donor : "Guest";

            var items = await _notificationService.GetRecentNotificationsAsync(userId, role, limit);

            var mapped = items.Select(n => new
            {
                n.Id,
                n.Title,
                n.Message,
                n.Category,
                n.Urgency,
                n.TargetUrl,
                n.IsRead,
                timeAgo = GetTimeAgo(n.CreatedAt)
            });

            return Ok(mapped);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.GetUserId();
            var role = User.IsInRole(Roles.Admin) ? Roles.Admin :
                       User.IsInRole(Roles.Donor) ? Roles.Donor : "Guest";

            var count = await _notificationService.GetUnreadCountAsync(userId, role);
            return Ok(new { count });
        }

        [HttpPost("mark-read/{id:int}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok(new { success = true });
        }

        [HttpPost("mark-all-read")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.GetUserId();
            var role = User.IsInRole(Roles.Admin) ? Roles.Admin :
                       User.IsInRole(Roles.Donor) ? Roles.Donor : "Guest";

            await _notificationService.MarkAllAsReadAsync(userId, role);
            return Ok(new { success = true });
        }

        private static string GetTimeAgo(DateTime utcTime)
        {
            var span = DateTime.UtcNow - utcTime;
            if (span.TotalMinutes < 1) return "Just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            if (span.TotalDays < 7) return $"{(int)span.TotalDays}d ago";
            return utcTime.ToString("dd MMM");
        }
    }
}
