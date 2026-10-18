using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BBDMS.Service.Interfaces;
using BBDMS.Model.Models.Entities;
using BBDMS.Model.Models.ViewModels;
using BBDMS.Web.Filters;

namespace BBDMS.Web.Controllers
{
    public class DonorController : Controller
    {
        private readonly IDonorService _donorService;
        private readonly IBloodGroupService _bloodGroupService;
        private readonly IBloodRequestService _bloodRequestService;
        private readonly IDonationService _donationService;
        private readonly IQrCodeService _qrCodeService;

        public DonorController(
            IDonorService donorService,
            IBloodGroupService bloodGroupService,
            IBloodRequestService bloodRequestService,
            IDonationService donationService,
            IQrCodeService qrCodeService)
        {
            _donorService = donorService;
            _bloodGroupService = bloodGroupService;
            _bloodRequestService = bloodRequestService;
            _donationService = donationService;
            _qrCodeService = qrCodeService;
        }

        // Donor Dashboard
        [DonorAuthorize]
        public async Task<IActionResult> Dashboard()
        {
            var donorId = User.GetUserId();
            if (donorId == null) return RedirectToAction("Login", "Account");

            var donor = await _donorService.GetDonorByIdAsync(donorId.Value);
            if (donor == null) return RedirectToAction("Login", "Account");

            var direct = (await _bloodRequestService.GetRequestsByDonorIdAsync(donorId.Value))?.ToList()
                         ?? new System.Collections.Generic.List<BloodRequest>();

            var community = (await _bloodRequestService.GetOpenCommunityRequestsAsync(donor.BloodGroup, 5)).ToList();
            var nearbyDonorCount = await _donorService.CountDonorsByBloodGroupAsync(donor.BloodGroup, donor.Id);

            var donations = (await _donationService.GetDonationsByDonorAsync(donorId.Value))?.ToList()
                            ?? new List<Donation>();

            var viewModel = new DonorDashboardViewModel
            {
                Donor = donor,
                DirectRequests = direct.Take(5).ToList(),
                CommunityRequests = community,
                NearbyDonors = nearbyDonorCount,
                PendingRequests = direct.Count(r => string.IsNullOrEmpty(r.Status) || r.Status == "Pending"),
                FulfilledRequests = direct.Count(r => r.Status == "Fulfilled" || r.Status == "Completed"),
                Donations = donations,
                TotalDonations = donations.Count,
                TotalUnitsDonated = donations.Sum(d => d.UnitsDonated),
                LastDonationDate = donations.Any() ? donations.Max(d => d.DonationDate) : donor.LastDonatedDate
            };

            return View(viewModel);
        }

        // My Blood Requests
        [DonorAuthorize]
        public async Task<IActionResult> MyRequests()
        {
            var donorId = User.GetUserId();
            if (donorId == null) return RedirectToAction("Login", "Account");

            var donor = await _donorService.GetDonorByIdAsync(donorId.Value);
            var direct = (await _bloodRequestService.GetRequestsByDonorIdAsync(donorId.Value))?.ToList()
                         ?? new System.Collections.Generic.List<BloodRequest>();

            if (donor != null)
            {
                ViewBag.CommunityRequests = await _bloodRequestService.GetOpenCommunityRequestsAsync(donor.BloodGroup);
                ViewBag.DonorBloodGroup = donor.BloodGroup;
            }
            ViewBag.Donor = donor;
            return View(direct);
        }

        // Donation History
        [DonorAuthorize]
        public async Task<IActionResult> DonationHistory()
        {
            var donorId = User.GetUserId();
            if (donorId == null) return RedirectToAction("Login", "Account");

            var donor = await _donorService.GetDonorByIdAsync(donorId.Value);
            if (donor == null) return RedirectToAction("Login", "Account");

            var donations = (await _donationService.GetDonationsByDonorAsync(donorId.Value))?.ToList()
                            ?? new List<Donation>();

            ViewBag.Donor = donor;
            ViewBag.TotalDonations = donations.Count;
            ViewBag.TotalUnits = donations.Sum(d => d.UnitsDonated);
            ViewBag.LastDonationDate = donations.Any() ? donations.Max(d => d.DonationDate) : (DateTime?)null;
            ViewBag.NextEligibleDate = donations.Any() ? donations.Max(d => d.DonationDate).AddDays(90) : (DateTime?)null;

            return View(donations);
        }

        // Log Donation
        [DonorAuthorize]
        public async Task<IActionResult> LogDonation()
        {
            var donorId = User.GetUserId();
            if (donorId == null) return RedirectToAction("Login", "Account");

            var donor = await _donorService.GetDonorByIdAsync(donorId.Value);
            if (donor == null) return RedirectToAction("Login", "Account");

            return View(new Donation
            {
                DonorId = donor.Id,
                BloodGroup = donor.BloodGroup,
                Location = donor.Address,
                DonationDate = DateTime.Today,
                UnitsDonated = 1
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DonorAuthorize]
        public async Task<IActionResult> LogDonation(Donation donation)
        {
            var donorId = User.GetUserId();
            if (donorId == null) return RedirectToAction("Login", "Account");

            var donor = await _donorService.GetDonorByIdAsync(donorId.Value);
            if (donor == null) return RedirectToAction("Login", "Account");

            if (donation.DonationDate > DateTime.Now)
            {
                ModelState.AddModelError(nameof(Donation.DonationDate), "Donation date cannot be in the future.");
            }

            if (!ModelState.IsValid)
            {
                donation.DonorId = donor.Id;
                return View(donation);
            }

            donation.DonorId = donor.Id;
            if (string.IsNullOrWhiteSpace(donation.BloodGroup)) donation.BloodGroup = donor.BloodGroup;

            await _donationService.AddDonationAsync(donation);
            TempData["Success"] = "Donation recorded. Your eligibility countdown has been updated from your donation history.";
            return RedirectToAction(nameof(DonationHistory));
        }

        // Delete Donation
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DonorAuthorize]
        public async Task<IActionResult> DeleteDonation(int id)
        {
            var donorId = User.GetUserId();
            if (donorId == null) return RedirectToAction("Login", "Account");

            var donation = await _donationService.GetDonationByIdAsync(id);
            if (donation != null && donation.DonorId == donorId.Value)
            {
                await _donationService.DeleteDonationAsync(id);
                TempData["Success"] = "Donation record removed.";
            }

            return RedirectToAction(nameof(DonationHistory));
        }

        private bool CanSeeDonorContact => User.Identity?.IsAuthenticated == true;

        // Nearby Donors
        public async Task<IActionResult> Nearby(string bloodGroup, string location, int page = 1)
        {
            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            ViewBag.BloodGroup = bloodGroup;
            ViewBag.Location = location;
            var donors = await _donorService.SearchDonorsAsync(bloodGroup, location, page, 12, CanSeeDonorContact);
            return View(donors);
        }

        // Donor Directory
        public async Task<IActionResult> Index(int page = 1)
        {
            var donors = await _donorService.SearchDonorsAsync(null, null, page, 12, CanSeeDonorContact);
            return View(donors);
        }

        // Search Donors
        public async Task<IActionResult> Search(string bloodGroup, string location, int page = 1)
        {
            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            ViewBag.BloodGroup = bloodGroup;
            ViewBag.Location = location;
            var donors = await _donorService.SearchDonorsAsync(bloodGroup, location, page, 12, CanSeeDonorContact);
            return View(donors);
        }

        // Contact Donor
        public async Task<IActionResult> Contact(int id)
        {
            var donor = await _donorService.GetDonorByIdAsync(id);
            if (donor == null)
            {
                return NotFound();
            }
            ViewBag.DonorId = id;
            ViewBag.Donor = donor;
            return View(new BloodRequest { BloodDonorID = id, BloodGroup = donor.BloodGroup, Location = donor.Address });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(BloodRequest request)
        {
            if (ModelState.IsValid)
            {
                if (request.BloodDonorID.HasValue && string.IsNullOrEmpty(request.BloodGroup))
                {
                    var donor = await _donorService.GetDonorByIdAsync(request.BloodDonorID.Value);
                    if (donor != null)
                    {
                        request.BloodGroup = donor.BloodGroup;
                        request.Location = donor.Address;
                    }
                }
                request.Status = "Pending";
                if (string.IsNullOrEmpty(request.Urgency)) request.Urgency = "Urgent";
                request.ApplyDate = System.DateTime.Now;
                await _bloodRequestService.SaveRequestAsync(request);
                TempData["Success"] = "Blood request sent to the donor. They will receive it in their portal.";
                return RedirectToAction("Index");
            }
            if (request.BloodDonorID.HasValue)
            {
                ViewBag.Donor = await _donorService.GetDonorByIdAsync(request.BloodDonorID.Value);
            }
            ViewBag.DonorId = request.BloodDonorID;
            return View(request);
        }

        // Digital Donor Card
        [DonorAuthorize]
        public async Task<IActionResult> DonorCard()
        {
            var donorId = User.GetUserId();
            if (donorId == null) return RedirectToAction("Login", "Account");

            var donor = await _donorService.GetDonorByIdAsync(donorId.Value);
            if (donor == null) return RedirectToAction("Login", "Account");

            var donations = (await _donationService.GetDonationsByDonorAsync(donorId.Value))?.ToList()
                            ?? new List<Donation>();

            var verificationUrl = Url.Action(nameof(VerifyDonor), "Donor", new { id = donor.Id }, Request.Scheme)
                                  ?? $"https://bbdms.org/verify/{donor.Id}";

            ViewBag.Donor = donor;
            ViewBag.TotalDonations = donations.Count;
            ViewBag.TotalUnits = donations.Sum(d => d.UnitsDonated);
            ViewBag.LastDonationDate = donations.Any() ? donations.Max(d => d.DonationDate) : donor.LastDonatedDate;
            ViewBag.QrCode = _qrCodeService.GeneratePngBase64(verificationUrl);
            ViewBag.VerificationUrl = verificationUrl;

            return View(donor);
        }

        // Donor Verification
        public async Task<IActionResult> VerifyDonor(int id)
        {
            var donor = await _donorService.GetDonorByIdAsync(id);
            if (donor == null || donor.Status != 1) return NotFound();

            var donations = (await _donationService.GetDonationsByDonorAsync(id))?.ToList()
                            ?? new List<Donation>();

            ViewBag.TotalDonations = donations.Count;
            ViewBag.LastDonationDate = donations.Any() ? donations.Max(d => d.DonationDate) : donor.LastDonatedDate;

            return View(donor);
        }

        // Appreciation Certificate
        [DonorAuthorize]
        public async Task<IActionResult> Certificate()
        {
            var donorId = User.GetUserId();
            if (donorId == null) return RedirectToAction("Login", "Account");

            var donor = await _donorService.GetDonorByIdAsync(donorId.Value);
            if (donor == null) return RedirectToAction("Login", "Account");

            var donations = (await _donationService.GetDonationsByDonorAsync(donorId.Value))?.ToList()
                            ?? new List<Donation>();

            var totalDonations = donations.Count;
            var totalUnits = donations.Sum(d => d.UnitsDonated);
            var milestone = GetMilestoneTier(totalDonations);

            var verificationUrl = Url.Action(nameof(VerifyDonor), "Donor", new { id = donor.Id }, Request.Scheme)
                                  ?? $"https://bbdms.org/verify/{donor.Id}";

            ViewBag.Donor = donor;
            ViewBag.TotalDonations = totalDonations;
            ViewBag.TotalUnits = totalUnits;
            ViewBag.Milestone = milestone;
            ViewBag.IssueDate = DateTime.Today.ToString("MMMM dd, yyyy");
            ViewBag.QrCode = _qrCodeService.GeneratePngBase64(verificationUrl);
            ViewBag.VerificationUrl = verificationUrl;

            return View(donor);
        }

        // Next Eligible Donation Calendar (.ics)
        [DonorAuthorize]
        public async Task<IActionResult> NextDonationCalendar()
        {
            var donorId = User.GetUserId();
            if (donorId == null) return RedirectToAction("Login", "Account");

            var donor = await _donorService.GetDonorByIdAsync(donorId.Value);
            if (donor == null) return RedirectToAction("Login", "Account");

            var lastDate = donor.LastDonatedDate ?? DateTime.Today;
            var nextDate = lastDate.AddDays(90);
            if (nextDate < DateTime.Today) nextDate = DateTime.Today;

            var startUtc = nextDate.Date.AddHours(9).ToUniversalTime().ToString("yyyyMMdd\\THHmmss\\Z");
            var endUtc = nextDate.Date.AddHours(10).ToUniversalTime().ToString("yyyyMMdd\\THHmmss\\Z");
            var nowUtc = DateTime.UtcNow.ToString("yyyyMMdd\\THHmmss\\Z");

            var icsContent = 
                "BEGIN:VCALENDAR\r\n" +
                "VERSION:2.0\r\n" +
                "PRODID:-//Blood Bank Management System//Donor Calendar//EN\r\n" +
                "CALSCALE:GREGORIAN\r\n" +
                "METHOD:PUBLISH\r\n" +
                "BEGIN:VEVENT\r\n" +
                $"UID:bbdms-donation-{donor.Id}-{nextDate:yyyyMMdd}@bbdms.org\r\n" +
                $"DTSTAMP:{nowUtc}\r\n" +
                $"DTSTART:{startUtc}\r\n" +
                $"DTEND:{endUtc}\r\n" +
                "SUMMARY:Blood Donation Day - Eligible to Save Lives!\r\n" +
                "DESCRIPTION:Your whole blood donation interval is complete. Visit your nearest Blood Bank or Hospital to donate.\\n\\nBlood Bank Management System\r\n" +
                "LOCATION:Nearest Blood Bank or Hospital\r\n" +
                "STATUS:CONFIRMED\r\n" +
                "END:VEVENT\r\n" +
                "END:VCALENDAR\r\n";

            var bytes = System.Text.Encoding.UTF8.GetBytes(icsContent);
            return File(bytes, "text/calendar", "BloodDonationReminder.ics");
        }

        // Donor Milestone Tier Helper
        private static string GetMilestoneTier(int totalDonations)
        {
            return totalDonations switch
            {
                >= 20 => "Platinum Life Saver Legend",
                >= 10 => "Gold Champion",
                >= 5 => "Silver Guardian",
                >= 2 => "Bronze Life Saver",
                1 => "First Drop Hero",
                _ => "Registered Life Saver"
            };
        }
    }
}
