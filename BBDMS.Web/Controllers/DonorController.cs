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
    }
}
