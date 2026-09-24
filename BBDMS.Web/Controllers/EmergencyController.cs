using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using BBDMS.Model.Models.Entities;
using BBDMS.Model.Models.ViewModels;
using BBDMS.Service.Interfaces;
using BBDMS.Web.Hubs;

namespace BBDMS.Web.Controllers
{
    public class EmergencyController : Controller
    {
        private readonly IHospitalService _hospitalService;
        private readonly IBloodBankService _bloodBankService;
        private readonly IAmbulanceService _ambulanceService;
        private readonly IOxygenService _oxygenService;
        private readonly IDonorService _donorService;
        private readonly IBloodRequestService _bloodRequestService;
        private readonly IBloodGroupService _bloodGroupService;
        private readonly IHubContext<EmergencyHub> _hubContext;

        public EmergencyController(
            IHospitalService hospitalService,
            IBloodBankService bloodBankService,
            IAmbulanceService ambulanceService,
            IOxygenService oxygenService,
            IDonorService donorService,
            IBloodRequestService bloodRequestService,
            IBloodGroupService bloodGroupService,
            IHubContext<EmergencyHub> hubContext)
        {
            _hospitalService = hospitalService;
            _bloodBankService = bloodBankService;
            _ambulanceService = ambulanceService;
            _oxygenService = oxygenService;
            _donorService = donorService;
            _bloodRequestService = bloodRequestService;
            _bloodGroupService = bloodGroupService;
            _hubContext = hubContext;
        }

        // 7.8 Nearby Emergency Services Hub
        public async Task<IActionResult> Index(string location, string serviceType = "All")
        {
            var viewModel = new EmergencySearchViewModel
            {
                Location = location,
                ServiceType = serviceType
            };

            if (serviceType == "All" || serviceType == "Hospitals")
            {
                viewModel.Hospitals = await _hospitalService.SearchHospitalsAsync(location, null, null);
            }

            if (serviceType == "All" || serviceType == "BloodBanks")
            {
                viewModel.BloodBanks = await _bloodBankService.SearchBloodBanksAsync(location, null);
            }

            if (serviceType == "All" || serviceType == "Ambulance")
            {
                viewModel.Ambulances = await _ambulanceService.SearchAmbulancesAsync(location, null, null);
            }

            if (serviceType == "All" || serviceType == "Oxygen")
            {
                viewModel.OxygenServices = await _oxygenService.SearchOxygenServicesAsync(location, null);
            }

            if (serviceType == "All" || serviceType == "Donors")
            {
                viewModel.Donors = await _donorService.SearchDonorsAsync(null, location);
            }

            if (serviceType == "All" || serviceType == "BloodRequests")
            {
                viewModel.EmergencyRequests = await _bloodRequestService.SearchEmergencyRequestsAsync(null, location);
            }

            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            return View(viewModel);
        }

        // 7.3 & 7.5 Hospitals, Emergency Beds and ICU
        public async Task<IActionResult> Hospitals(string location, bool? icuOnly, bool? emergencyBedOnly)
        {
            ViewBag.Location = location;
            ViewBag.IcuOnly = icuOnly;
            ViewBag.EmergencyBedOnly = emergencyBedOnly;

            var hospitals = await _hospitalService.SearchHospitalsAsync(location, icuOnly, emergencyBedOnly);
            return View(hospitals);
        }

        // 7.4 Blood Bank Information
        public async Task<IActionResult> BloodBanks(string location, string bloodGroup)
        {
            ViewBag.Location = location;
            ViewBag.BloodGroup = bloodGroup;
            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();

            var bloodBanks = await _bloodBankService.SearchBloodBanksAsync(location, bloodGroup);
            return View(bloodBanks);
        }

        // 7.6 Ambulance Service Directory
        public async Task<IActionResult> Ambulances(string location, string ambulanceType, bool? onlyAvailable)
        {
            ViewBag.Location = location;
            ViewBag.AmbulanceType = ambulanceType;
            ViewBag.OnlyAvailable = onlyAvailable;

            var ambulances = await _ambulanceService.SearchAmbulancesAsync(location, ambulanceType, onlyAvailable);
            return View(ambulances);
        }

        // 7.6 Submit Emergency Ambulance Request (GET)
        public async Task<IActionResult> RequestAmbulance(int? serviceId)
        {
            if (serviceId.HasValue)
            {
                var service = await _ambulanceService.GetAmbulanceByIdAsync(serviceId.Value);
                if (service != null)
                {
                    ViewBag.SelectedProvider = service.ProviderName;
                    ViewBag.AmbulanceType = service.AmbulanceType;
                }
            }

            return View(new AmbulanceRequest
            {
                AmbulanceServiceId = serviceId,
                Urgency = "Emergency"
            });
        }

        // 7.6 Submit Emergency Ambulance Request (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("emergencyLimiter")]
        public async Task<IActionResult> RequestAmbulance(AmbulanceRequest request)
        {
            if (ModelState.IsValid)
            {
                await _ambulanceService.SubmitAmbulanceRequestAsync(request);

                // Broadcast live SignalR alert to dispatchers
                await _hubContext.Clients.All.SendAsync("ReceiveAmbulanceAlert", new
                {
                    patientName = request.PatientName,
                    pickupLocation = request.PickupAddress,
                    contactNumber = request.ContactNumber,
                    urgency = request.Urgency,
                    timestamp = System.DateTime.Now.ToString("hh:mm tt")
                });

                TempData["Success"] = "Ambulance request submitted successfully! An emergency dispatcher will call you immediately.";
                return RedirectToAction(nameof(Ambulances));
            }

            return View(request);
        }

        // 7.7 Oxygen Services Directory
        public async Task<IActionResult> Oxygen(string location, bool? homeDeliveryOnly)
        {
            ViewBag.Location = location;
            ViewBag.HomeDeliveryOnly = homeDeliveryOnly;

            var services = await _oxygenService.SearchOxygenServicesAsync(location, homeDeliveryOnly);
            return View(services);
        }

        // 7.2 Emergency Blood Requests Board
        public async Task<IActionResult> BloodRequests(string bloodGroup, string location)
        {
            ViewBag.Location = location;
            ViewBag.BloodGroup = bloodGroup;
            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();

            var requests = await _bloodRequestService.SearchEmergencyRequestsAsync(bloodGroup, location);
            return View(requests);
        }

        // 7.2 Create Emergency Blood Request (GET)
        public async Task<IActionResult> CreateBloodRequest(int? donorId)
        {
            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            if (donorId.HasValue)
            {
                var donor = await _donorService.GetDonorByIdAsync(donorId.Value);
                if (donor != null)
                {
                    ViewBag.Donor = donor;
                }
            }

            return View(new BloodRequest
            {
                BloodDonorID = donorId,
                Urgency = "Critical",
                UnitsRequired = 1
            });
        }

        // 7.2 Create Emergency Blood Request (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("emergencyLimiter")]
        public async Task<IActionResult> CreateBloodRequest(BloodRequest request)
        {
            if (ModelState.IsValid)
            {
                request.Status = "Pending";
                request.ApplyDate = System.DateTime.Now;
                await _bloodRequestService.SaveRequestAsync(request);

                // Broadcast live SignalR alert to connected users and admin dispatchers
                await _hubContext.Clients.All.SendAsync("ReceiveBloodRequestAlert", new
                {
                    patientName = request.Name,
                    bloodGroup = request.BloodGroup ?? request.BloodRequireFor,
                    hospital = request.HospitalName ?? "Emergency Facility",
                    urgency = request.Urgency ?? "Critical",
                    units = request.UnitsRequired,
                    timestamp = System.DateTime.Now.ToString("hh:mm tt")
                });

                TempData["Success"] = "Emergency Blood Request posted successfully! Registered donors and community lifesavers have been notified.";
                return RedirectToAction(nameof(BloodRequests));
            }

            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            return View(request);
        }
    }
}
