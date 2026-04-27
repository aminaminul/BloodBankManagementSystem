using Microsoft.AspNetCore.Mvc;
using BBDMS.Service.Interfaces;
using BBDMS.Model.Models.Entities;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;

namespace BBDMS.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IDonorService _donorService;
        private readonly IBloodGroupService _bloodGroupService;
        private readonly IBloodRequestService _bloodRequestService;

        public AccountController(IDonorService donorService, IBloodGroupService bloodGroupService, IBloodRequestService bloodRequestService)
        {
            _donorService = donorService;
            _bloodGroupService = bloodGroupService;
            _bloodRequestService = bloodRequestService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var donors = await _donorService.GetAllDonorsAsync();
            var donor = donors.FirstOrDefault(d => d.EmailId == email && d.Password == password);
            if (donor != null)
            {
                HttpContext.Session.SetInt32("bbdmsdid", donor.Id);
                return RedirectToAction("Profile");
            }
            TempData["Error"] = "Invalid email or password.";
            return View();
        }

        public async Task<IActionResult> Register()
        {
            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(BloodDonor donor)
        {
            if (ModelState.IsValid)
            {
                donor.Status = 1;
                await _donorService.RegisterDonorAsync(donor);
                TempData["Success"] = "Registration successful! You can now login.";
                return RedirectToAction("Login");
            }
            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            return View(donor);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Profile()
        {
            var donorId = HttpContext.Session.GetInt32("bbdmsdid");
            if (donorId == null) return RedirectToAction("Login");
            
            var donor = await _donorService.GetDonorByIdAsync(donorId.Value);
            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            return View(donor);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(BloodDonor donor)
        {
            var donorId = HttpContext.Session.GetInt32("bbdmsdid");
            if (donorId == null) return RedirectToAction("Login");

            var existingDonor = await _donorService.GetDonorByIdAsync(donorId.Value);
            if(existingDonor != null)
            {
                existingDonor.FullName = donor.FullName;
                existingDonor.MobileNumber = donor.MobileNumber;
                existingDonor.Age = donor.Age;
                existingDonor.Gender = donor.Gender;
                existingDonor.BloodGroup = donor.BloodGroup;
                existingDonor.Address = donor.Address;
                existingDonor.Message = donor.Message;
                await _donorService.UpdateDonorAsync(existingDonor);
                TempData["Success"] = "Profile has been updated.";
            }

            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            return View(existingDonor);
        }

        public IActionResult ChangePassword()
        {
            var donorId = HttpContext.Session.GetInt32("bbdmsdid");
            if (donorId == null) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var donorId = HttpContext.Session.GetInt32("bbdmsdid");
            if (donorId == null) return RedirectToAction("Login");

            var donor = await _donorService.GetDonorByIdAsync(donorId.Value);
            if (donor != null && donor.Password == currentPassword)
            {
                donor.Password = newPassword;
                await _donorService.UpdateDonorAsync(donor);
                TempData["Success"] = "Password changed successfully.";
            }
            else
            {
                TempData["Error"] = "Invalid current password.";
            }
            return View();
        }

        public async Task<IActionResult> RequestReceived()
        {
            var donorId = HttpContext.Session.GetInt32("bbdmsdid");
            if (donorId == null) return RedirectToAction("Login");

            var requests = await _bloodRequestService.GetRequestsByDonorIdAsync(donorId.Value);
            return View(requests);
        }
    }
}
