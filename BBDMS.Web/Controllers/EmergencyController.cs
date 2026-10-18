using System;
using System.Linq;
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
        private readonly IPageService _pageService;
        private readonly IHubContext<EmergencyHub> _hubContext;
        private readonly INotificationService _notificationService;

        public EmergencyController(
            IHospitalService hospitalService,
            IBloodBankService bloodBankService,
            IAmbulanceService ambulanceService,
            IOxygenService oxygenService,
            IDonorService donorService,
            IBloodRequestService bloodRequestService,
            IBloodGroupService bloodGroupService,
            IPageService pageService,
            IHubContext<EmergencyHub> hubContext,
            INotificationService notificationService)
        {
            _hospitalService = hospitalService;
            _bloodBankService = bloodBankService;
            _ambulanceService = ambulanceService;
            _oxygenService = oxygenService;
            _donorService = donorService;
            _bloodRequestService = bloodRequestService;
            _bloodGroupService = bloodGroupService;
            _pageService = pageService;
            _hubContext = hubContext;
            _notificationService = notificationService;
        }

        // Emergency Services Hub
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
                viewModel.Donors = (await _donorService.SearchDonorsAsync(
                    null, location, 1, 12, User.Identity?.IsAuthenticated == true)).Items;
            }

            if (serviceType == "All" || serviceType == "BloodRequests")
            {
                viewModel.EmergencyRequests = await _bloodRequestService.SearchEmergencyRequestsAsync(null, location);
            }

            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            return View(viewModel);
        }

        // Healthcare Facility Map
        public async Task<IActionResult> Map()
        {
            ViewBag.Hospitals = await _hospitalService.GetAllHospitalsAsync();
            ViewBag.Ambulances = await _ambulanceService.GetAllAmbulancesAsync();
            ViewBag.BloodBanks = await _bloodBankService.GetAllBloodBanksAsync();
            return View();
        }

        // Map Data Feed
        [HttpGet]
        public async Task<IActionResult> MapData()
        {
            var hospitals = (await _hospitalService.GetAllHospitalsAsync())?.Select(h => new
            {
                type = "hospital",
                id = h.Id,
                name = h.Name,
                address = h.Address,
                city = h.City,
                phone = h.EmergencyHelpline ?? h.ContactNumber,
                emergencyBeds = h.AvailableEmergencyBeds,
                icuBeds = h.AvailableIcuBeds,
                services = h.EmergencyServices
            });

            var ambulances = (await _ambulanceService.GetAllAmbulancesAsync())?.Select(a => new
            {
                type = "ambulance",
                id = a.Id,
                name = a.ProviderName,
                address = a.Location,
                city = a.Location,
                phone = a.ContactNumber,
                ambulanceType = a.AmbulanceType,
                isAvailable = a.IsAvailable
            });

            var bloodBanks = (await _bloodBankService.GetAllBloodBanksAsync())?.Select(b => new
            {
                type = "bloodbank",
                id = b.Id,
                name = b.Name,
                address = b.Address,
                city = b.City,
                phone = b.ContactNumber,
                bloodStock = b.AvailableBloodGroups
            });

            return Json(new
            {
                hospitals,
                ambulances,
                bloodBanks
            });
        }

        // Hospitals & Bed Availability
        public async Task<IActionResult> Hospitals(string location, bool? icuOnly, bool? emergencyBedOnly)
        {
            ViewBag.Location = location;
            ViewBag.IcuOnly = icuOnly;
            ViewBag.EmergencyBedOnly = emergencyBedOnly;

            var hospitals = await _hospitalService.SearchHospitalsAsync(location, icuOnly, emergencyBedOnly);
            return View(hospitals);
        }

        // Blood Banks
        public async Task<IActionResult> BloodBanks(string location, string bloodGroup)
        {
            ViewBag.Location = location;
            ViewBag.BloodGroup = bloodGroup;
            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();

            var bloodBanks = await _bloodBankService.SearchBloodBanksAsync(location, bloodGroup);
            return View(bloodBanks);
        }

        // Ambulance Directory
        public async Task<IActionResult> Ambulances(string location, string ambulanceType, bool? onlyAvailable)
        {
            ViewBag.Location = location;
            ViewBag.AmbulanceType = ambulanceType;
            ViewBag.OnlyAvailable = onlyAvailable;

            var ambulances = await _ambulanceService.SearchAmbulancesAsync(location, ambulanceType, onlyAvailable);
            return View(ambulances);
        }

        // Ambulance Dispatch Booking
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("emergencyLimiter")]
        public async Task<IActionResult> RequestAmbulance(AmbulanceRequest request)
        {
            if (ModelState.IsValid)
            {
                await _ambulanceService.SubmitAmbulanceRequestAsync(request);

                await _hubContext.Clients.Group(EmergencyHub.DispatcherGroup).SendAsync("ReceiveAmbulanceAlert", new
                {
                    id = request.Id,
                    trackingToken = request.TrackingToken,
                    patientName = request.PatientName,
                    pickupLocation = request.PickupAddress,
                    contactNumber = request.ContactNumber,
                    urgency = request.Urgency,
                    timestamp = System.DateTime.Now.ToString("hh:mm tt")
                });

                await _notificationService.CreateNotificationAsync(new Notification
                {
                    Title = "🚑 Emergency Ambulance Booked",
                    Message = $"Pickup at {request.PickupAddress}. Patient: {request.PatientName}. Urgency: {request.Urgency}.",
                    Category = "Ambulance",
                    Urgency = "P0_Critical",
                    TargetRole = "Admin",
                    TargetUrl = $"/Emergency/TrackAmbulance?token={request.TrackingToken}"
                });

                TempData["Success"] = "Ambulance request submitted successfully! An emergency dispatcher will call you immediately.";
                return RedirectToAction(nameof(TrackAmbulance), new { token = request.TrackingToken });
            }

            return View(request);
        }

        // Oxygen Services
        public async Task<IActionResult> Oxygen(string location, bool? homeDeliveryOnly)
        {
            ViewBag.Location = location;
            ViewBag.HomeDeliveryOnly = homeDeliveryOnly;

            var services = await _oxygenService.SearchOxygenServicesAsync(location, homeDeliveryOnly);
            return View(services);
        }

        // Blood Requests Board
        public async Task<IActionResult> BloodRequests(string bloodGroup, string location)
        {
            ViewBag.Location = location;
            ViewBag.BloodGroup = bloodGroup;
            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();

            var requests = await _bloodRequestService.SearchEmergencyRequestsAsync(bloodGroup, location);
            return View(requests);
        }

        // Create Blood Request
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

                await _hubContext.Clients.All.SendAsync("ReceiveBloodRequestAlert", new
                {
                    id = request.ID,
                    token = request.TrackingToken,
                    bloodGroup = request.BloodGroup ?? "Unspecified",
                    hospital = request.HospitalName ?? "Emergency Facility",
                    urgency = request.Urgency ?? "Critical",
                    units = request.UnitsRequired,
                    timestamp = System.DateTime.Now.ToString("hh:mm tt")
                });

                await _hubContext.Clients.Group(EmergencyHub.DispatcherGroup).SendAsync("ReceiveBloodRequestAlertFull", new
                {
                    id = request.ID,
                    token = request.TrackingToken,
                    patientName = request.Name,
                    bloodGroup = request.BloodGroup ?? "Unspecified",
                    hospital = request.HospitalName ?? "Emergency Facility",
                    contactNumber = request.ContactNumber,
                    urgency = request.Urgency ?? "Critical",
                    units = request.UnitsRequired,
                    timestamp = System.DateTime.Now.ToString("hh:mm tt")
                });

                await _notificationService.CreateNotificationAsync(new Notification
                {
                    Title = $"🚨 Urgent: {request.UnitsRequired} Unit(s) of {request.BloodGroup} Needed",
                    Message = $"Required at {request.HospitalName ?? "Emergency Facility"}. Urgency: {request.Urgency ?? "Critical"}.",
                    Category = "Emergency",
                    Urgency = "P0_Critical",
                    TargetRole = "All",
                    TargetUrl = $"/Emergency/TrackRequest?token={request.TrackingToken}"
                });

                TempData["Success"] = "Emergency Blood Request posted successfully! Registered donors and community lifesavers have been notified.";
                return RedirectToAction(nameof(TrackRequest), new { token = request.TrackingToken });
            }

            ViewBag.BloodGroups = await _bloodGroupService.GetAllGroupsAsync();
            return View(request);
        }

        // Blood Request Details
        public async Task<IActionResult> RequestDetails(int? id, string? token)
        {
            BloodRequest? request = null;
            if (!string.IsNullOrWhiteSpace(token))
            {
                request = await _bloodRequestService.GetRequestByTokenAsync(token);
            }
            else if (id.HasValue)
            {
                request = await _bloodRequestService.GetRequestByIdAsync(id.Value);
            }

            if (request == null) return NotFound();

            if (!string.IsNullOrEmpty(request.HospitalName))
            {
                var hospitals = await _hospitalService.GetAllHospitalsAsync();
                ViewBag.Hospital = hospitals.FirstOrDefault(h =>
                    !string.IsNullOrEmpty(h.Name) &&
                    h.Name.Contains(request.HospitalName, System.StringComparison.OrdinalIgnoreCase));
            }

            return View(request);
        }

        // Blood Request Tracking
        public async Task<IActionResult> TrackRequest(string? token, int? id, string? contact)
        {
            BloodRequest? request = null;

            if (!string.IsNullOrWhiteSpace(token))
            {
                request = await _bloodRequestService.GetRequestByTokenAsync(token);
            }
            else if (id.HasValue)
            {
                request = await _bloodRequestService.GetRequestByIdAsync(id.Value);
            }
            else if (!string.IsNullOrWhiteSpace(contact))
            {
                request = await _bloodRequestService.GetLatestRequestByContactAsync(contact);
            }

            ViewBag.Contact = contact;
            ViewBag.Token = token;
            return View(request);
        }

        // Ambulance Dispatch Tracking
        public async Task<IActionResult> TrackAmbulance(string? token, string? contact)
        {
            var request = await FindAmbulanceRequestAsync(token, contact);

            if (request?.AmbulanceServiceId != null)
            {
                ViewBag.Provider = await _ambulanceService.GetAmbulanceByIdAsync(request.AmbulanceServiceId.Value);
            }

            ViewBag.Contact = string.IsNullOrWhiteSpace(contact) ? request?.ContactNumber : contact;
    
            return View(request);
        }

        // Ambulance Status Feed
        [HttpGet]
        public async Task<IActionResult> AmbulanceStatus(string? token)
        {
            var request = await _ambulanceService.GetAmbulanceRequestByTokenAsync(token ?? string.Empty);
            if (request == null) return NotFound();

            var status = AmbulanceTracking.NormalizeStatus(request.Status);
            return Json(new
            {
                id = request.Id,
                status,
                statusLabel = AmbulanceTracking.StatusLabel(status),
                badge = AmbulanceTracking.StatusBadge(status),
                icon = AmbulanceTracking.StatusIcon(status),
                eta = AmbulanceTracking.EtaText(status),
                progress = AmbulanceTracking.ProgressPercent(status),
                isActive = AmbulanceTracking.IsActive(status),
                patientName = request.PatientName,
                updatedAt = DateTime.Now.ToString("hh:mm:ss tt"),
                steps = AmbulanceTracking.BuildSteps(request)
            });
        }

        // Emergency Contact & Dispatch Helpline
        public async Task<IActionResult> AmbulanceContact(string? token, string? contact, string? location)
        {
            var request = await FindAmbulanceRequestAsync(token, contact);
            AmbulanceService? provider = null;

            if (request?.AmbulanceServiceId != null)
            {
                provider = await _ambulanceService.GetAmbulanceByIdAsync(request.AmbulanceServiceId.Value);
            }

            var searchLocation = !string.IsNullOrWhiteSpace(location) ? location : provider?.Location;

            ViewBag.Request = request;
            ViewBag.Provider = provider;
            ViewBag.ContactInfo = await _pageService.GetContactInfoAsync();
            ViewBag.NearbyAmbulances = (await _ambulanceService.SearchAmbulancesAsync(searchLocation, null, null)
                                        ?? Enumerable.Empty<AmbulanceService>()).Take(6).ToList();
            ViewBag.Location = searchLocation;

            return View();
        }

        // Helper: Ambulance Request Lookup
        private async Task<AmbulanceRequest?> FindAmbulanceRequestAsync(string? token, string? contact)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                var byToken = await _ambulanceService.GetAmbulanceRequestByTokenAsync(token);
                if (byToken != null) return byToken;
            }

            if (!string.IsNullOrWhiteSpace(contact))
            {
                return await _ambulanceService.GetLatestAmbulanceRequestByContactAsync(contact);
            }

            return null;
        }
    }
}
