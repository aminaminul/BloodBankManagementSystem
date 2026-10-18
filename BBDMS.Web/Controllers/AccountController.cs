using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using BBDMS.Service.Interfaces;
using BBDMS.Web.Filters;
using BBDMS.Model.Models.Entities;
using BBDMS.Model.Models.ViewModels;
using BBDMS.Service.Common;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;

namespace BBDMS.Web.Controllers
{
    public class AccountController : Controller
    {
        private const int MinPasswordLength = 8;

        private readonly IDonorService _donorService;
        private readonly IBloodGroupService _bloodGroupService;
        private readonly IBloodRequestService _bloodRequestService;
        private readonly IPasswordResetService _passwordResetService;

        public AccountController(
            IDonorService donorService,
            IBloodGroupService bloodGroupService,
            IBloodRequestService bloodRequestService,
            IPasswordResetService passwordResetService)
        {
            _donorService = donorService;
            _bloodGroupService = bloodGroupService;
            _bloodRequestService = bloodRequestService;
            _passwordResetService = passwordResetService;
        }

        // Login
        public IActionResult Login()
        {
            if (User.IsInRole(Roles.Donor))
                return RedirectToAction("Dashboard", "Donor");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("authLimiter")]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Please provide both email and password.";
                return View();
            }

            var donor = await _donorService.GetDonorByEmailAsync(email);

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

                await SignInAsync(donor.Id, donor.FullName ?? "Donor", donor.EmailId ?? string.Empty, Roles.Donor);

                if (!string.IsNullOrWhiteSpace(donor.ThemePreference))
                {
                    BBDMS.Web.Common.ThemePreference.ApplyCookie(HttpContext, donor.ThemePreference);
                }

                return RedirectToAction("Dashboard", "Donor");
            }

            TempData["Error"] = "Invalid email address or password.";
            return View();
        }

        // Register
        public async Task<IActionResult> Register()
        {
            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(DonorRegisterViewModel registration)
        {
            if (!string.IsNullOrWhiteSpace(registration.EmailId))
            {
                var existing = await _donorService.GetDonorByEmailAsync(registration.EmailId);
                if (existing != null)
                {
                    ModelState.AddModelError("EmailId", "This email address is already registered.");
                }
            }

            if (string.IsNullOrWhiteSpace(registration.Password) || registration.Password.Length < MinPasswordLength)
            {
                ModelState.AddModelError("Password", $"Password must be at least {MinPasswordLength} characters.");
            }

            if (ModelState.IsValid)
            {
                var donor = new BloodDonor
                {
                    FullName = registration.FullName.Trim(),
                    MobileNumber = registration.MobileNumber.Trim(),
                    EmailId = registration.EmailId.Trim(),
                    Gender = registration.Gender,
                    Age = registration.Age,
                    BloodGroup = registration.BloodGroup,
                    Address = registration.Address.Trim(),
                    Message = registration.Message,
                    IsAvailable = registration.IsAvailable,
                    Password = PasswordHasher.HashPassword(registration.Password),
                    Status = 1,
                    PostingDate = System.DateTime.Now
                };

                try
                {
                    await _donorService.RegisterDonorAsync(donor);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                {
                    ModelState.AddModelError("EmailId", "This email address is already registered.");
                    ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
                    return View(registration);
                }

                TempData["Success"] = "Registration successful! You can now login with your credentials.";
                return RedirectToAction("Login");
            }

            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            return View(registration);
        }

        // Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Donor Profile
        [BBDMS.Web.Filters.DonorAuthorize]
        public async Task<IActionResult> Profile()
        {
            var donorId = User.GetUserId();
            if (donorId == null) return RedirectToAction("Login");
            
            var donor = await _donorService.GetDonorByIdAsync(donorId.Value);
            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            return View(donor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [BBDMS.Web.Filters.DonorAuthorize]
        public async Task<IActionResult> Profile(DonorProfileUpdateViewModel update)
        {
            var donorId = User.GetUserId();
            if (donorId == null) return RedirectToAction("Login");

            var existingDonor = await _donorService.GetDonorByIdAsync(donorId.Value);
            if (existingDonor == null) return RedirectToAction("Login");

            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();

            if (!ModelState.IsValid)
            {
                return View(Redisplay(existingDonor, update));
            }

            existingDonor.FullName = update.FullName.Trim();
            existingDonor.MobileNumber = update.MobileNumber.Trim();
            existingDonor.Age = update.Age;
            existingDonor.Gender = update.Gender;
            existingDonor.BloodGroup = update.BloodGroup;
            existingDonor.Address = update.Address.Trim();
            existingDonor.Message = update.Message;
            existingDonor.IsAvailable = update.IsAvailable;

            await _donorService.UpdateDonorAsync(existingDonor);
            TempData["Success"] = "Profile has been updated. Your donation eligibility is derived from your donation history.";

            return RedirectToAction(nameof(Profile));
        }

        // Change Password
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
            var donorId = User.GetUserId();
            if (donorId == null) return RedirectToAction("Login");

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < MinPasswordLength)
            {
                TempData["Error"] = $"New password must be at least {MinPasswordLength} characters.";
                return View();
            }

            if (string.Equals(currentPassword, newPassword, System.StringComparison.Ordinal))
            {
                TempData["Error"] = "The new password must be different from the current password.";
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

        // Requests Received
        [BBDMS.Web.Filters.DonorAuthorize]
        public async Task<IActionResult> RequestReceived()
        {
            var donorId = User.GetUserId();
            if (donorId == null) return RedirectToAction("Login");

            var donor = await _donorService.GetDonorByIdAsync(donorId.Value);
            var directRequests = await _bloodRequestService.GetRequestsByDonorIdAsync(donorId.Value);

            if (donor != null)
            {
                ViewBag.CommunityRequests = await _bloodRequestService.GetOpenCommunityRequestsAsync(donor.BloodGroup);
                ViewBag.DonorBloodGroup = donor.BloodGroup;
            }

            return View(directRequests);
        }

        // Password Reset Workflow
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            if (User.IsInRole(Roles.Donor)) return RedirectToAction("Dashboard", "Donor");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("authLimiter")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Please enter your registered email address.";
                return View();
            }

            var donor = await _donorService.GetDonorByEmailAsync(email);
            if (donor != null && donor.Status == 1)
            {
                var token = await _passwordResetService.CreateResetTokenAsync(email);
                var resetLink = Url.Action(nameof(ResetPassword), "Account", new { token }, Request.Scheme)
                                ?? $"/Account/ResetPassword?token={token}";

                ViewBag.Email = email;
                ViewBag.ResetLink = resetLink;
                TempData["Success"] = "Password reset instructions have been generated.";
                return View("ForgotPasswordConfirmation");
            }

            ViewBag.Email = email;
            TempData["Success"] = "If an account exists with that email address, a password reset link has been dispatched.";
            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Invalid or expired password reset token.";
                return RedirectToAction(nameof(Login));
            }

            var resetToken = await _passwordResetService.ValidateTokenAsync(token);
            if (resetToken == null)
            {
                TempData["Error"] = "This password reset token has expired or has already been used.";
                return RedirectToAction(nameof(Login));
            }

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("authLimiter")]
        public async Task<IActionResult> ResetPassword(string token, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Invalid password reset request.";
                return RedirectToAction(nameof(Login));
            }

            var resetToken = await _passwordResetService.ValidateTokenAsync(token);
            if (resetToken == null)
            {
                TempData["Error"] = "This password reset token has expired or has already been used.";
                return RedirectToAction(nameof(Login));
            }

            ViewBag.Token = token;

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < MinPasswordLength)
            {
                ModelState.AddModelError("newPassword", $"Password must be at least {MinPasswordLength} characters.");
                return View();
            }

            if (!string.Equals(newPassword, confirmPassword, System.StringComparison.Ordinal))
            {
                ModelState.AddModelError("confirmPassword", "The passwords do not match.");
                return View();
            }

            var donor = await _donorService.GetDonorByEmailAsync(resetToken.Email);
            if (donor != null)
            {
                donor.Password = PasswordHasher.HashPassword(newPassword);
                await _donorService.UpdateDonorAsync(donor);
                await _passwordResetService.MarkTokenAsUsedAsync(token);

                TempData["Success"] = "Your password has been successfully reset! You can now log in with your new password.";
                return RedirectToAction(nameof(Login));
            }

            TempData["Error"] = "Unable to reset password for this account.";
            return RedirectToAction(nameof(Login));
        }

        // Helpers
        private async Task SignInAsync(int userId, string displayName, string email, string role)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, displayName),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }

        private static BloodDonor Redisplay(BloodDonor existing, DonorProfileUpdateViewModel update)
        {
            existing.FullName = update.FullName;
            existing.MobileNumber = update.MobileNumber;
            existing.Age = update.Age;
            existing.Gender = update.Gender;
            existing.BloodGroup = update.BloodGroup;
            existing.Address = update.Address;
            existing.Message = update.Message;
            existing.IsAvailable = update.IsAvailable;
            return existing;
        }
    }
}
