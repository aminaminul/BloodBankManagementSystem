using Microsoft.AspNetCore.Mvc;
using BBDMS.Service.Interfaces;
using BBDMS.Model.Models.Entities;
using System.Threading.Tasks;

namespace BBDMS.Web.Controllers
{
    public class DonorController : Controller
    {
        private readonly IDonorService _donorService;
        private readonly IBloodGroupService _bloodGroupService;
        private readonly IBloodRequestService _bloodRequestService;

        public DonorController(IDonorService donorService, IBloodGroupService bloodGroupService, IBloodRequestService bloodRequestService)
        {
            _donorService = donorService;
            _bloodGroupService = bloodGroupService;
            _bloodRequestService = bloodRequestService;
        }

        public async Task<IActionResult> Index()
        {
            var donors = await _donorService.GetAllDonorsAsync();
            return View(donors);
        }

        public async Task<IActionResult> Search(string bloodGroup, string location)
        {
            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            var donors = await _donorService.SearchDonorsAsync(bloodGroup, location);
            return View(donors);
        }

        public async Task<IActionResult> Contact(int id)
        {
            var donor = await _donorService.GetDonorByIdAsync(id);
            if (donor == null)
            {
                return NotFound();
            }
            ViewBag.DonorId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(BloodRequest request)
        {
            if (ModelState.IsValid)
            {
                await _bloodRequestService.SaveRequestAsync(request);
                TempData["Success"] = "Request has been sent. We will contact you shortly.";
                return RedirectToAction("Index");
            }
            ViewBag.DonorId = request.BloodDonorID;
            return View(request);
        }
    }
}
