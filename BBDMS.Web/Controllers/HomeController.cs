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
        private readonly IPageService _pageService;
        private readonly IBloodGroupService _bloodGroupService;
        private readonly IHomeSummaryService _homeSummaryService;

        public HomeController(
            ILogger<HomeController> logger,
            IPageService pageService,
            IBloodGroupService bloodGroupService,
            IHomeSummaryService homeSummaryService)
        {
            _logger = logger;
            _pageService = pageService;
            _bloodGroupService = bloodGroupService;
            _homeSummaryService = homeSummaryService;
        }

        // Home
        public async Task<IActionResult> Index()
        {
            var viewModel = await _homeSummaryService.GetSummaryAsync();
            viewModel.BloodGroups = (await _bloodGroupService.GetAllGroupsAsync()) ?? Enumerable.Empty<BloodGroup>();
            viewModel.ContactInfo = await _pageService.GetContactInfoAsync();

            return View(viewModel);
        }

        // About Us
        public async Task<IActionResult> About()
        {
            var pages = await _pageService.GetAllPagesAsync();
            ViewBag.AllPages = pages;
            var aboutPage = pages.FirstOrDefault(p => p.Type == "aboutus") ?? await _pageService.GetPageByTypeAsync("aboutus");
            return View(aboutPage);
        }

        // Contact Us
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

        // Access Denied (403)
        public IActionResult AccessDenied()
        {
            Response.StatusCode = 403;
            ViewBag.Code = 403;
            ViewBag.Message = "You do not have permission to open that page.";
            return View("Status");
        }

        // HTTP Status Handler (400, 403, 404, 429, 500)
        public IActionResult Status(int code)
        {
            Response.Headers.CacheControl = "no-store";
            Response.StatusCode = code;
            ViewBag.Code = code;
            ViewBag.Message = code switch
            {
                400 => "That request could not be understood. Please go back and try again.",
                403 => "You do not have permission to open that page.",
                404 => "We could not find the page you were looking for.",
                429 => "Too many requests from this device. Please wait a moment and retry.",
                _ => "Something went wrong while handling that request."
            };
            return View();
        }

        // Blood Compatibility Matrix & Eligibility Quiz
        public IActionResult Compatibility()
        {
            return View();
        }

        // Privacy Policy
        public IActionResult Privacy()
        {
            return View();
        }

        // Error Handler
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
