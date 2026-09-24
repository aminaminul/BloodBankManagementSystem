using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BBDMS.Model.Models.Entities;
using BBDMS.Model.Models.ViewModels;
using BBDMS.Service.Interfaces;

namespace BBDMS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDonorService _donorService;
        private readonly IPageService _pageService;
        private readonly IBloodGroupService _bloodGroupService;
        private readonly IBloodRequestService _bloodRequestService;
        private readonly IHospitalService _hospitalService;
        private readonly IBloodBankService _bloodBankService;
        private readonly IAmbulanceService _ambulanceService;
        private readonly IOxygenService _oxygenService;

        public HomeController(
            ILogger<HomeController> logger,
            IDonorService donorService,
            IPageService pageService,
            IBloodGroupService bloodGroupService,
            IBloodRequestService bloodRequestService,
            IHospitalService hospitalService,
            IBloodBankService bloodBankService,
            IAmbulanceService ambulanceService,
            IOxygenService oxygenService)
        {
            _logger = logger;
            _donorService = donorService;
            _pageService = pageService;
            _bloodGroupService = bloodGroupService;
            _bloodRequestService = bloodRequestService;
            _hospitalService = hospitalService;
            _bloodBankService = bloodBankService;
            _ambulanceService = ambulanceService;
            _oxygenService = oxygenService;
        }

        public async Task<IActionResult> Index()
        {
            var donors = (await _donorService.GetAllDonorsAsync()) ?? Enumerable.Empty<BloodDonor>();
            var hospitals = (await _hospitalService.GetAllHospitalsAsync()) ?? Enumerable.Empty<Hospital>();
            var bloodBanks = (await _bloodBankService.GetAllBloodBanksAsync()) ?? Enumerable.Empty<BloodBank>();
            var ambulances = (await _ambulanceService.GetAllAmbulancesAsync()) ?? Enumerable.Empty<AmbulanceService>();
            var oxygenServices = (await _oxygenService.GetAllOxygenServicesAsync()) ?? Enumerable.Empty<OxygenService>();
            var requests = (await _bloodRequestService.GetAllRequestsAsync()) ?? Enumerable.Empty<BloodRequest>();
            var bloodGroups = (await _bloodGroupService.GetAllGroupsAsync()) ?? Enumerable.Empty<BloodGroup>();
            var contactInfo = await _pageService.GetContactInfoAsync();

            var viewModel = new HomeIndexViewModel
            {
                TotalDonors = donors.Count(),
                TotalHospitals = hospitals.Count(),
                TotalAvailableIcuBeds = hospitals.Sum(h => h.AvailableIcuBeds),
                TotalAvailableEmergencyBeds = hospitals.Sum(h => h.AvailableEmergencyBeds),
                TotalBloodBanks = bloodBanks.Count(),
                TotalAmbulances = ambulances.Count(),
                TotalOxygenSuppliers = oxygenServices.Count(),
                TotalActiveRequests = requests.Count(r => r.Status != "Fulfilled" && r.Status != "Cancelled"),

                ContactInfo = contactInfo,
                BloodGroups = bloodGroups,
                ActiveDonors = donors.Where(d => d.Status == 1).OrderByDescending(d => d.Id).Take(6).ToList(),
                UrgentRequests = requests.Where(r => r.Status != "Fulfilled" && r.Status != "Cancelled").OrderByDescending(r => r.ID).Take(3).ToList(),
                FeaturedHospitals = hospitals.Take(3).ToList(),
                FeaturedBloodBanks = bloodBanks.Take(3).ToList(),
                FeaturedAmbulances = ambulances.Where(a => a.IsAvailable).Take(3).ToList(),
                FeaturedOxygenSuppliers = oxygenServices.Take(3).ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> About()
        {
            var pages = await _pageService.GetAllPagesAsync();
            ViewBag.AllPages = pages;
            var aboutPage = pages.FirstOrDefault(p => p.Type == "aboutus") ?? await _pageService.GetPageByTypeAsync("aboutus");
            return View(aboutPage);
        }

        public async Task<IActionResult> Contact()
        {
            var contactInfo = await _pageService.GetContactInfoAsync();
            return View(contactInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactQuery query)
        {
            if (ModelState.IsValid)
            {
                await _pageService.SaveContactQueryAsync(query);
                TempData["Message"] = "Your message has been sent successfully!";
                return RedirectToAction("Contact");
            }
            return View(await _pageService.GetContactInfoAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
