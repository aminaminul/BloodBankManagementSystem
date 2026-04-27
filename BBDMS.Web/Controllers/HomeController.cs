using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BBDMS.Model.Models.ViewModels;
using BBDMS.Service.Interfaces;
using BBDMS.Model.Models.Entities;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace BBDMS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDonorService _donorService;
        private readonly IPageService _pageService;

        public HomeController(ILogger<HomeController> logger, IDonorService donorService, IPageService pageService)
        {
            _logger = logger;
            _donorService = donorService;
            _pageService = pageService;
        }

        public async Task<IActionResult> Index()
        {
            var donors = await _donorService.GetAllDonorsAsync();
            var randomDonors = donors.OrderBy(x => Guid.NewGuid()).Take(6).ToList();
            return View(randomDonors);
        }

        public async Task<IActionResult> About()
        {
            var page = await _pageService.GetPageByTypeAsync("aboutus");
            return View(page);
        }

        public async Task<IActionResult> Contact()
        {
            var contactInfo = await _pageService.GetContactInfoAsync();
            return View(contactInfo);
        }

        [HttpPost]
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
