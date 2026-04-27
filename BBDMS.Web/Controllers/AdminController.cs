using Microsoft.AspNetCore.Mvc;
using BBDMS.Service.Interfaces;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BBDMS.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IDonorService _donorService;

        public AdminController(IAdminService adminService, IDonorService donorService)
        {
            _adminService = adminService;
            _donorService = donorService;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("adminId") != null)
                return RedirectToAction("Dashboard");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string username, string password)
        {
            var admin = await _adminService.LoginAsync(username, password);
            if (admin != null)
            {
                HttpContext.Session.SetInt32("adminId", admin.ID);
                return RedirectToAction("Dashboard");
            }
            TempData["Error"] = "Invalid details";
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            if (HttpContext.Session.GetInt32("adminId") == null) return RedirectToAction("Index");
            
            ViewBag.TotalDonors = await _adminService.GetTotalDonorsCountAsync();
            ViewBag.TotalRequests = await _adminService.GetTotalRequestsCountAsync();
            
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("adminId");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DonorList()
        {
            if (HttpContext.Session.GetInt32("adminId") == null) return RedirectToAction("Index");
            var donors = await _donorService.GetAllDonorsAsync();
            return View(donors);
        }
    }
}
