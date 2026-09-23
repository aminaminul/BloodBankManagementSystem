using Microsoft.AspNetCore.Mvc;
using BBDMS.Service.Interfaces;
using BBDMS.Model.Models.Entities;
using BBDMS.Service.Common;
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
            if (HttpContext.Session.GetInt32("bbdmsdid") != null)
                return RedirectToAction(nameof(Profile));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Please provide both email and password.";
                return View();
            }

            var donors = await _donorService.GetAllDonorsAsync();
            var donor = donors.FirstOrDefault(d => string.Equals(d.EmailId, email.Trim(), System.StringComparison.OrdinalIgnoreCase));

            if (donor != null && PasswordHasher.VerifyPassword(password, donor.Password, out bool needsRehash))
            {
                if (donor.Status != 1)
                {
                    TempData["Error"] = "Your donor account is currently disabled or pending review.";
                    return View();
                }

                if (needsRehash)
                {
                    donor.Password = PasswordHasher.HashPassword(password);
                    await _donorService.UpdateDonorAsync(donor);
                }

                HttpContext.Session.SetInt32("bbdmsdid", donor.Id);
                return RedirectToAction("Profile");
            }

            TempData["Error"] = "Invalid email address or password.";
            return View();
        }

        public async Task<IActionResult> Register()
        {
            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(BloodDonor donor)
        {
            // Check for duplicate email
            if (!string.IsNullOrWhiteSpace(donor.EmailId))
            {
                var existing = (await _donorService.GetAllDonorsAsync())
                    .FirstOrDefault(d => string.Equals(d.EmailId, donor.EmailId.Trim(), System.StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    ModelState.AddModelError("EmailId", "This email address is already registered.");
                }
            }

            if (string.IsNullOrWhiteSpace(donor.Password) || donor.Password.Length < 4)
            {
                ModelState.AddModelError("Password", "Password must be at least 4 characters.");
            }

            if (ModelState.IsValid)
            {
                donor.EmailId = donor.EmailId.Trim();
                donor.Password = PasswordHasher.HashPassword(donor.Password);
                donor.Status = 1;
                donor.PostingDate = System.DateTime.Now;
                await _donorService.RegisterDonorAsync(donor);
                TempData["Success"] = "Registration successful! You can now login with your credentials.";
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

        [BBDMS.Web.Filters.DonorAuthorize]
        public async Task<IActionResult> Profile()
        {
            var donorId = HttpContext.Session.GetInt32("bbdmsdid");
            if (donorId == null) return RedirectToAction("Login");
            
            var donor = await _donorService.GetDonorByIdAsync(donorId.Value);
            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            return View(donor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [BBDMS.Web.Filters.DonorAuthorize]
        public async Task<IActionResult> Profile(BloodDonor donor)
        {
            var donorId = HttpContext.Session.GetInt32("bbdmsdid");
            if (donorId == null) return RedirectToAction("Login");

            var existingDonor = await _donorService.GetDonorByIdAsync(donorId.Value);
            if (existingDonor != null)
            {
                existingDonor.FullName = donor.FullName;
                existingDonor.MobileNumber = donor.MobileNumber;
                existingDonor.Age = donor.Age;
                existingDonor.Gender = donor.Gender;
                existingDonor.BloodGroup = donor.BloodGroup;
                existingDonor.Address = donor.Address;
                existingDonor.Message = donor.Message;
                existingDonor.IsAvailable = donor.IsAvailable;
                existingDonor.LastDonatedDate = donor.LastDonatedDate;
                await _donorService.UpdateDonorAsync(existingDonor);
                TempData["Success"] = "Profile has been updated.";
            }

            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            return View(existingDonor);
        }

        [BBDMS.Web.Filters.DonorAuthorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [BBDMS.Web.Filters.DonorAuthorize]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var donorId = HttpContext.Session.GetInt32("bbdmsdid");
            if (donorId == null) return RedirectToAction("Login");

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 4)
            {
                TempData["Error"] = "New password must be at least 4 characters.";
                return View();
            }

            var donor = await _donorService.GetDonorByIdAsync(donorId.Value);
            if (donor != null && PasswordHasher.VerifyPassword(currentPassword, donor.Password, out _))
            {
                donor.Password = PasswordHasher.HashPassword(newPassword);
                await _donorService.UpdateDonorAsync(donor);
                TempData["Success"] = "Password changed successfully.";
            }
            else
            {
                TempData["Error"] = "Invalid current password.";
            }
            return View();
        }

        [BBDMS.Web.Filters.DonorAuthorize]
        public async Task<IActionResult> RequestReceived()
        {
            var donorId = HttpContext.Session.GetInt32("bbdmsdid");
            if (donorId == null) return RedirectToAction("Login");

            var donor = await _donorService.GetDonorByIdAsync(donorId.Value);
            var directRequests = await _bloodRequestService.GetRequestsByDonorIdAsync(donorId.Value);

            if (donor != null)
            {
                var communityRequests = (await _bloodRequestService.GetAllRequestsAsync())
                    .Where(r => r.BloodDonorID == null && (r.BloodGroup == donor.BloodGroup || r.BloodRequireFor == donor.BloodGroup))
                    .ToList();
                ViewBag.CommunityRequests = communityRequests;
                ViewBag.DonorBloodGroup = donor.BloodGroup;
            }

            return View(directRequests);
        }
    }
}
