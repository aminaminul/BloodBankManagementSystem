using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BBDMS.Model.Models.Entities;
using BBDMS.Service.Interfaces;

namespace BBDMS.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IDonorService _donorService;
        private readonly IHospitalService _hospitalService;
        private readonly IBloodBankService _bloodBankService;
        private readonly IAmbulanceService _ambulanceService;
        private readonly IOxygenService _oxygenService;
        private readonly IBloodRequestService _bloodRequestService;
        private readonly IPageService _pageService;

        public AdminController(
            IAdminService adminService,
            IDonorService donorService,
            IHospitalService hospitalService,
            IBloodBankService bloodBankService,
            IAmbulanceService ambulanceService,
            IOxygenService oxygenService,
            IBloodRequestService bloodRequestService,
            IPageService pageService)
        {
            _adminService = adminService;
            _donorService = donorService;
            _hospitalService = hospitalService;
            _bloodBankService = bloodBankService;
            _ambulanceService = ambulanceService;
            _oxygenService = oxygenService;
            _bloodRequestService = bloodRequestService;
            _pageService = pageService;
        }

        private bool IsAdminLoggedIn => HttpContext.Session.GetInt32("adminId") != null;

        public IActionResult Index()
        {
            if (IsAdminLoggedIn)
                return RedirectToAction(nameof(Dashboard));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string username, string password)
        {
            var admin = await _adminService.LoginAsync(username, password);
            if (admin != null)
            {
                HttpContext.Session.SetInt32("adminId", admin.ID);
                return RedirectToAction(nameof(Dashboard));
            }
            TempData["Error"] = "Invalid administrator username or password.";
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));

            ViewBag.TotalDonors = await _adminService.GetTotalDonorsCountAsync();
            ViewBag.TotalRequests = await _adminService.GetTotalRequestsCountAsync();
            ViewBag.TotalHospitals = await _adminService.GetTotalHospitalsCountAsync();
            ViewBag.TotalIcuBeds = await _adminService.GetTotalAvailableIcuBedsAsync();
            ViewBag.TotalAmbulances = await _adminService.GetTotalAmbulancesCountAsync();
            ViewBag.TotalOxygenSuppliers = await _adminService.GetTotalOxygenSuppliersCountAsync();
            ViewBag.TotalBloodBanks = await _adminService.GetTotalBloodBanksCountAsync();

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("adminId");
            return RedirectToAction(nameof(Index));
        }

        #region Donor Management
        public async Task<IActionResult> DonorList()
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            var donors = await _donorService.GetAllDonorsAsync();
            return View(donors);
        }

        public async Task<IActionResult> ToggleDonorAvailability(int id)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            await _donorService.ToggleAvailabilityAsync(id);
            TempData["Success"] = "Donor availability status updated.";
            return RedirectToAction(nameof(DonorList));
        }

        public async Task<IActionResult> ToggleDonorStatus(int id)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            await _donorService.ToggleStatusAsync(id);
            TempData["Success"] = "Donor active/inactive status updated.";
            return RedirectToAction(nameof(DonorList));
        }

        public async Task<IActionResult> DeleteDonor(int id)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            await _donorService.DeleteDonorAsync(id);
            TempData["Success"] = "Donor deleted successfully.";
            return RedirectToAction(nameof(DonorList));
        }
        #endregion

        #region Hospital & ICU Management
        public async Task<IActionResult> Hospitals()
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            var hospitals = await _hospitalService.GetAllHospitalsAsync();
            return View(hospitals);
        }

        public IActionResult HospitalCreate()
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            return View(new Hospital());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HospitalCreate(Hospital hospital)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            if (ModelState.IsValid)
            {
                await _hospitalService.AddHospitalAsync(hospital);
                TempData["Success"] = "Hospital added successfully.";
                return RedirectToAction(nameof(Hospitals));
            }
            return View(hospital);
        }

        public async Task<IActionResult> HospitalEdit(int id)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            var hospital = await _hospitalService.GetHospitalByIdAsync(id);
            if (hospital == null) return NotFound();
            return View(hospital);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HospitalEdit(Hospital hospital)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            if (ModelState.IsValid)
            {
                await _hospitalService.UpdateHospitalAsync(hospital);
                TempData["Success"] = "Hospital & ICU bed information updated.";
                return RedirectToAction(nameof(Hospitals));
            }
            return View(hospital);
        }

        public async Task<IActionResult> HospitalDelete(int id)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            await _hospitalService.DeleteHospitalAsync(id);
            TempData["Success"] = "Hospital removed successfully.";
            return RedirectToAction(nameof(Hospitals));
        }
        #endregion

        #region Blood Bank Management
        public async Task<IActionResult> BloodBanks()
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            var banks = await _bloodBankService.GetAllBloodBanksAsync();
            return View(banks);
        }

        public IActionResult BloodBankCreate()
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            return View(new BloodBank());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BloodBankCreate(BloodBank bank)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            if (ModelState.IsValid)
            {
                await _bloodBankService.AddBloodBankAsync(bank);
                TempData["Success"] = "Blood Bank added successfully.";
                return RedirectToAction(nameof(BloodBanks));
            }
            return View(bank);
        }

        public async Task<IActionResult> BloodBankEdit(int id)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            var bank = await _bloodBankService.GetBloodBankByIdAsync(id);
            if (bank == null) return NotFound();
            return View(bank);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BloodBankEdit(BloodBank bank)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            if (ModelState.IsValid)
            {
                await _bloodBankService.UpdateBloodBankAsync(bank);
                TempData["Success"] = "Blood Bank updated successfully.";
                return RedirectToAction(nameof(BloodBanks));
            }
            return View(bank);
        }

        public async Task<IActionResult> BloodBankDelete(int id)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            await _bloodBankService.DeleteBloodBankAsync(id);
            TempData["Success"] = "Blood Bank deleted successfully.";
            return RedirectToAction(nameof(BloodBanks));
        }
        #endregion

        #region Ambulance Management & Requests
        public async Task<IActionResult> Ambulances()
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            var ambulances = await _ambulanceService.GetAllAmbulancesAsync();
            return View(ambulances);
        }

        public IActionResult AmbulanceCreate()
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            return View(new AmbulanceService());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AmbulanceCreate(AmbulanceService ambulance)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            if (ModelState.IsValid)
            {
                await _ambulanceService.AddAmbulanceAsync(ambulance);
                TempData["Success"] = "Ambulance fleet provider added.";
                return RedirectToAction(nameof(Ambulances));
            }
            return View(ambulance);
        }

        public async Task<IActionResult> AmbulanceEdit(int id)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            var ambulance = await _ambulanceService.GetAmbulanceByIdAsync(id);
            if (ambulance == null) return NotFound();
            return View(ambulance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AmbulanceEdit(AmbulanceService ambulance)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            if (ModelState.IsValid)
            {
                await _ambulanceService.UpdateAmbulanceAsync(ambulance);
                TempData["Success"] = "Ambulance information updated.";
                return RedirectToAction(nameof(Ambulances));
            }
            return View(ambulance);
        }

        public async Task<IActionResult> AmbulanceDelete(int id)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            await _ambulanceService.DeleteAmbulanceAsync(id);
            TempData["Success"] = "Ambulance removed.";
            return RedirectToAction(nameof(Ambulances));
        }

        public async Task<IActionResult> AmbulanceRequests()
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            var requests = await _ambulanceService.GetAllAmbulanceRequestsAsync();
            return View(requests);
        }

        public async Task<IActionResult> UpdateAmbulanceRequestStatus(int id, string status)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            await _ambulanceService.UpdateAmbulanceRequestStatusAsync(id, status);
            TempData["Success"] = $"Ambulance request status updated to {status}.";
            return RedirectToAction(nameof(AmbulanceRequests));
        }
        #endregion

        #region Oxygen Service Management
        public async Task<IActionResult> Oxygen()
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            var services = await _oxygenService.GetAllOxygenServicesAsync();
            return View(services);
        }

        public IActionResult OxygenCreate()
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            return View(new OxygenService());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OxygenCreate(OxygenService oxygenService)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            if (ModelState.IsValid)
            {
                await _oxygenService.AddOxygenServiceAsync(oxygenService);
                TempData["Success"] = "Oxygen service provider added.";
                return RedirectToAction(nameof(Oxygen));
            }
            return View(oxygenService);
        }

        public async Task<IActionResult> OxygenEdit(int id)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            var service = await _oxygenService.GetOxygenServiceByIdAsync(id);
            if (service == null) return NotFound();
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OxygenEdit(OxygenService oxygenService)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            if (ModelState.IsValid)
            {
                await _oxygenService.UpdateOxygenServiceAsync(oxygenService);
                TempData["Success"] = "Oxygen service updated.";
                return RedirectToAction(nameof(Oxygen));
            }
            return View(oxygenService);
        }

        public async Task<IActionResult> OxygenDelete(int id)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            await _oxygenService.DeleteOxygenServiceAsync(id);
            TempData["Success"] = "Oxygen service removed.";
            return RedirectToAction(nameof(Oxygen));
        }
        #endregion

        #region Blood Request Management
        public async Task<IActionResult> BloodRequests()
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            var requests = await _bloodRequestService.GetAllRequestsAsync();
            return View(requests);
        }

        public async Task<IActionResult> UpdateRequestStatus(int id, string status)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            await _bloodRequestService.UpdateRequestStatusAsync(id, status);
            TempData["Success"] = $"Blood request status updated to {status}.";
            return RedirectToAction(nameof(BloodRequests));
        }
        #endregion

        #region User Contact Queries & Website Information Management
        public async Task<IActionResult> ContactQueries()
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            var queries = await _pageService.GetAllContactQueriesAsync();
            return View(queries);
        }

        public async Task<IActionResult> DeleteContactQuery(int id)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            await _pageService.DeleteContactQueryAsync(id);
            TempData["Success"] = "Contact query removed.";
            return RedirectToAction(nameof(ContactQueries));
        }

        public async Task<IActionResult> ContactInfo()
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            var info = await _pageService.GetContactInfoAsync();
            return View(info);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactInfo(ContactInfo info)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            if (ModelState.IsValid)
            {
                await _pageService.UpdateContactInfoAsync(info);
                TempData["Success"] = "Website contact details updated successfully in database.";
                return RedirectToAction(nameof(ContactInfo));
            }
            return View(info);
        }

        public async Task<IActionResult> Pages()
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            var pages = await _pageService.GetAllPagesAsync();
            return View(pages);
        }

        public async Task<IActionResult> PageEdit(int id)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            var page = await _pageService.GetPageByIdAsync(id);
            if (page == null) return NotFound();
            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PageEdit(PageContent pageContent)
        {
            if (!IsAdminLoggedIn) return RedirectToAction(nameof(Index));
            if (ModelState.IsValid)
            {
                await _pageService.UpdatePageContentAsync(pageContent);
                TempData["Success"] = $"Page '{pageContent.PageName}' content updated in database.";
                return RedirectToAction(nameof(Pages));
            }
            return View(pageContent);
        }
        #endregion
    }
}
