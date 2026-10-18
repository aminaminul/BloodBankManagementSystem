using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Model.Models.ViewModels;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BBDMS.Service.Services
{
    // Admin Analytics & Report Service
    public class ReportService : IReportService
    {
        private readonly IRepository<BloodDonor> _donorRepository;
        private readonly IRepository<BloodRequest> _requestRepository;
        private readonly IRepository<Donation> _donationRepository;
        private readonly IAdminService _adminService;

        public ReportService(
            IRepository<BloodDonor> donorRepository,
            IRepository<BloodRequest> requestRepository,
            IRepository<Donation> donationRepository,
            IAdminService adminService)
        {
            _donorRepository = donorRepository;
            _requestRepository = requestRepository;
            _donationRepository = donationRepository;
            _adminService = adminService;
        }

        public async Task<AdminReportsViewModel> BuildAsync(DateTime today)
        {
            var seriesStart = new DateTime(today.Year, today.Month, 1).AddMonths(-11);
            var thirtyDaysAgo = today.AddDays(-30);

            var labels = new List<string>();
            var unitsPerMonth = new List<int>();
            var donationsPerMonth = new List<int>();
            var requestsPerMonth = new List<int>();
            var newDonorsPerMonth = new List<int>();

            var donationTotals = await _donationRepository.Query()
                .Where(d => d.DonationDate >= seriesStart)
                .GroupBy(d => new { d.DonationDate.Year, d.DonationDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count(), Units = g.Sum(x => x.UnitsDonated) })
                .ToListAsync();

            var requestTotals = await _requestRepository.Query()
                .Where(r => r.ApplyDate != null && r.ApplyDate >= seriesStart)
                .GroupBy(r => new { r.ApplyDate!.Value.Year, r.ApplyDate!.Value.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync();

            var donorTotals = await _donorRepository.Query()
                .Where(d => d.PostingDate >= seriesStart)
                .GroupBy(d => new { d.PostingDate.Year, d.PostingDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync();

            for (var i = 0; i < 12; i++)
            {
                var month = seriesStart.AddMonths(i);
                labels.Add(month.ToString("MMM yy"));

                var donationBucket = donationTotals.FirstOrDefault(d => d.Year == month.Year && d.Month == month.Month);
                donationsPerMonth.Add(donationBucket?.Count ?? 0);
                unitsPerMonth.Add(donationBucket?.Units ?? 0);
                requestsPerMonth.Add(requestTotals.FirstOrDefault(r => r.Year == month.Year && r.Month == month.Month)?.Count ?? 0);
                newDonorsPerMonth.Add(donorTotals.FirstOrDefault(d => d.Year == month.Year && d.Month == month.Month)?.Count ?? 0);
            }

            var donationQuery = _donationRepository.Query();
            var requestQuery = _requestRepository.Query();
            var donorQuery = _donorRepository.Query();

            return new AdminReportsViewModel
            {
                TotalDonors = await donorQuery.CountAsync(),
                TotalRequests = await requestQuery.CountAsync(),
                TotalHospitals = await _adminService.GetTotalHospitalsCountAsync(),
                TotalBloodBanks = await _adminService.GetTotalBloodBanksCountAsync(),
                TotalAmbulances = await _adminService.GetTotalAmbulancesCountAsync(),
                TotalOxygen = await _adminService.GetTotalOxygenSuppliersCountAsync(),

                FulfilledRequests = await requestQuery.CountAsync(r => r.Status == "Fulfilled" || r.Status == "Completed"),
                PendingRequests = await requestQuery.CountAsync(r => r.Status == null || r.Status == "" || r.Status == "Pending"),
                RejectedRequests = await requestQuery.CountAsync(r => r.Status == "Rejected"),
                EmergencyRequests = await requestQuery.CountAsync(r => r.Urgency != null && r.Urgency.Contains("Critical")),

                AvailableDonors = await donorQuery.CountAsync(d => d.IsAvailable),
                TotalDonations = await donationQuery.CountAsync(),
                TotalUnitsDonated = await donationQuery.SumAsync(d => d.UnitsDonated),
                DonationsLast30Days = await donationQuery.CountAsync(d => d.DonationDate >= thirtyDaysAgo),
                RequestsLast30Days = await requestQuery.CountAsync(r => r.ApplyDate != null && r.ApplyDate >= thirtyDaysAgo),

                Donations = await donationQuery.OrderByDescending(d => d.DonationDate).Take(50).ToListAsync(),
                Donors = await donorQuery.OrderByDescending(d => d.Id).Take(50).ToListAsync(),
                Requests = await requestQuery.OrderByDescending(r => r.ID).Take(50).ToListAsync(),

                SeriesLabels = labels,
                UnitsPerMonth = unitsPerMonth,
                DonationsPerMonth = donationsPerMonth,
                RequestsPerMonth = requestsPerMonth,
                NewDonorsPerMonth = newDonorsPerMonth,
                SeriesStart = seriesStart
            };
        }
    }
}
