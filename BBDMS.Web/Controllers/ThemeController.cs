using System.Threading.Tasks;
using BBDMS.Service.Interfaces;
using BBDMS.Web.Common;
using BBDMS.Web.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BBDMS.Web.Controllers
{
    public class ThemeController : Controller
    {
        private readonly IDonorService _donorService;
        private readonly IAdminService _adminService;

        public ThemeController(IDonorService donorService, IAdminService adminService)
        {
            _donorService = donorService;
            _adminService = adminService;
        }

        // Theme Switcher (Dark / Light)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Set(string theme, string returnUrl)
        {
            var normalized = ThemePreference.Normalize(theme);
            ThemePreference.ApplyCookie(HttpContext, normalized);

            var persisted = false;
            var userId = User.GetUserId();

            if (userId.HasValue && User.IsInRole(BBDMS.Web.Filters.Roles.Donor))
            {
                var donor = await _donorService.GetDonorByIdAsync(userId.Value);
                if (donor != null && donor.ThemePreference != normalized)
                {
                    donor.ThemePreference = normalized;
                    await _donorService.UpdateDonorAsync(donor);
                    persisted = true;
                }
            }
            else if (userId.HasValue && User.IsInRole(BBDMS.Web.Filters.Roles.Admin))
            {
                var admin = await _adminService.GetAdminByIdAsync(userId.Value);
                if (admin != null && admin.ThemePreference != normalized)
                {
                    admin.ThemePreference = normalized;
                    await _adminService.UpdateAdminAsync(admin);
                    persisted = true;
                }
            }

            if (WantsJson())
            {
                return Json(new { theme = normalized, persisted });
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        private bool WantsJson()
        {
            var accept = Request.Headers["Accept"].ToString();
            return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                || accept.Contains("application/json", System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
