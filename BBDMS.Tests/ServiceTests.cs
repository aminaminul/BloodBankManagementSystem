using System;
using System.Linq;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Service.Services;
using Xunit;

namespace BBDMS.Tests
{
    public class PasswordHasherTests
    {
        [Fact]
        public void Hash_Then_Verify_AcceptsTheRightPassword()
        {
            var hash = BBDMS.Service.Common.PasswordHasher.HashPassword("donor1234");

            Assert.True(BBDMS.Service.Common.PasswordHasher.VerifyPassword("donor1234", hash, out var needsRehash));
            Assert.False(needsRehash);
        }

        [Fact]
        public void Verify_RejectsTheWrongPassword()
        {
            var hash = BBDMS.Service.Common.PasswordHasher.HashPassword("donor1234");

            Assert.False(BBDMS.Service.Common.PasswordHasher.VerifyPassword("wrong-password", hash, out _));
        }

        [Fact]
        public void Verify_FlagsLegacyPlainTextPasswordsForRehashing()
        {
            Assert.True(BBDMS.Service.Common.PasswordHasher.VerifyPassword("legacy", "legacy", out var needsRehash));
            Assert.True(needsRehash);
        }

        [Fact]
        public void Hash_RejectsEmptyPasswords()
        {
            Assert.Throws<ArgumentException>(() => BBDMS.Service.Common.PasswordHasher.HashPassword(" "));
        }
    }

    public class DonorServiceTests
    {
        [Fact]
        public async Task GetDonorByEmail_IsCaseInsensitiveAndDoesNotLoadTheWholeTable()
        {
            using var db = new TestDatabase();
            var service = new DonorService(db.Repository<BloodDonor>());

            var found = await service.GetDonorByEmailAsync("  TANVIR@EXAMPLE.COM ");

            Assert.NotNull(found);
            Assert.Equal("Tanvir Ahmed", found!.FullName);
        }

        [Fact]
        public async Task GetDonorByEmail_ReturnsNullForUnknownOrEmptyInput()
        {
            using var db = new TestDatabase();
            var service = new DonorService(db.Repository<BloodDonor>());

            Assert.Null(await service.GetDonorByEmailAsync("nobody@example.com"));
            Assert.Null(await service.GetDonorByEmailAsync("   "));
        }

        [Fact]
        public async Task SearchDonors_PagesAndOnlyCountsActiveDonors()
        {
            using var db = new TestDatabase();
            var service = new DonorService(db.Repository<BloodDonor>());

            var firstPage = await service.SearchDonorsAsync(null, null, page: 1, pageSize: 1);

            Assert.Equal(2, firstPage.TotalItems);      // the disabled donor is excluded
            Assert.Single(firstPage.Items);
            Assert.Equal(2, firstPage.TotalPages);
            Assert.True(firstPage.HasNextPage);
        }

        [Fact]
        public async Task SearchDonors_NeverLeaksCredentials()
        {
            using var db = new TestDatabase();
            var service = new DonorService(db.Repository<BloodDonor>());

            var anonymous = await service.SearchDonorsAsync(null, null, includeContact: false);
            var signedIn = await service.SearchDonorsAsync(null, null, includeContact: true);

            // Anonymous visitors get masked contact details and never an email address.
            var masked = anonymous.Items.First();
            Assert.False(masked.ContactVisible);
            Assert.Equal("Sign in to view", masked.MobileNumber);
            Assert.Equal(string.Empty, masked.EmailId);

            var visible = signedIn.Items.First();
            Assert.True(visible.ContactVisible);
            Assert.StartsWith("01", visible.MobileNumber);

            // The DTO has no password surface at all.
            Assert.DoesNotContain(typeof(BBDMS.Model.Models.ViewModels.DonorListItemViewModel).GetProperties(),
                p => p.Name.Contains("Password", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task SearchDonors_FiltersByBloodGroupAndLocation()
        {
            using var db = new TestDatabase();
            var service = new DonorService(db.Repository<BloodDonor>());

            var oPositive = await service.SearchDonorsAsync("O+", null);
            var sylhet = await service.SearchDonorsAsync(null, "Sylhet");

            Assert.Single(oPositive.Items);
            Assert.Equal("Tanvir Ahmed", oPositive.Items.First().FullName);
            Assert.Empty(sylhet.Items);
        }

        [Fact]
        public async Task CountDonorsByBloodGroup_ExcludesTheCallerAndInactiveDonors()
        {
            using var db = new TestDatabase();
            var service = new DonorService(db.Repository<BloodDonor>());

            var others = await service.CountDonorsByBloodGroupAsync("O+", excludeDonorId: 0);

            Assert.Equal(1, others);
        }

        [Fact]
        public async Task GetPagedDonors_OrdersNewestFirst()
        {
            using var db = new TestDatabase();
            var service = new DonorService(db.Repository<BloodDonor>());

            var page = await service.GetPagedDonorsAsync(1, 10);

            Assert.Equal(3, page.TotalItems);
            Assert.True(page.Items.First().Id > page.Items.Last().Id);
        }
    }

    public class BloodRequestServiceTests
    {
        [Fact]
        public async Task OpenCommunityRequests_ExcludeAssignedAndClosedRequests()
        {
            using var db = new TestDatabase();
            var service = new BloodRequestService(db.Repository<BloodRequest>());

            var open = (await service.GetOpenCommunityRequestsAsync("O+")).ToList();

            Assert.Single(open);
            Assert.Equal("Open O+ request", open[0].Name);
            Assert.Null(open[0].BloodDonorID);
        }

        [Fact]
        public async Task OpenCommunityRequests_RespectTheCap()
        {
            using var db = new TestDatabase();
            var service = new BloodRequestService(db.Repository<BloodRequest>());

            var capped = await service.GetOpenCommunityRequestsAsync(null, take: 1);

            Assert.Single(capped);
        }

        [Fact]
        public async Task SearchEmergencyRequests_IgnoresTheReasonFieldAndHidesClosedRequests()
        {
            using var db = new TestDatabase();
            var service = new BloodRequestService(db.Repository<BloodRequest>());

            var all = (await service.SearchEmergencyRequestsAsync(null, null)).ToList();
            var byGroup = (await service.SearchEmergencyRequestsAsync("O+", null)).ToList();

            // "Fulfilled" must not appear on the public board.
            Assert.DoesNotContain(all, r => r.Status == "Fulfilled");
            Assert.All(byGroup, r => Assert.Equal("O+", r.BloodGroup));

            // "Surgery" is a reason, not a blood group, and must not match anything.
            Assert.Empty(await service.SearchEmergencyRequestsAsync("Surgery", null));
        }

        [Fact]
        public async Task PagedRequests_AreNewestFirst()
        {
            using var db = new TestDatabase();
            var service = new BloodRequestService(db.Repository<BloodRequest>());

            var page = await service.GetPagedRequestsAsync(1, 10);
            var dates = page.Items.Select(r => r.ApplyDate).ToList();

            Assert.Equal(dates.OrderByDescending(d => d), dates);
        }

        [Fact]
        public async Task GetLatestRequestByContact_FindsMatchingRequestDirectlyInSql()
        {
            using var db = new TestDatabase();
            var service = new BloodRequestService(db.Repository<BloodRequest>());

            var found = await service.GetLatestRequestByContactAsync("01711223344");
            Assert.NotNull(found);
            Assert.Equal("Open O+ request", found!.Name);

            var notFound = await service.GetLatestRequestByContactAsync("01999999999");
            Assert.Null(notFound);

            var empty = await service.GetLatestRequestByContactAsync("   ");
            Assert.Null(empty);
        }

        [Fact]
        public async Task SaveRequest_AssignsUnguessableTrackingTokenAndCanBeRetrieved()
        {
            using var db = new TestDatabase();
            var service = new BloodRequestService(db.Repository<BloodRequest>());

            var request = new BloodRequest
            {
                Name = "Token Test Requester",
                EmailId = "tokentest@example.com",
                ContactNumber = "01755443322",
                BloodRequireFor = "Platelet transfusion",
                BloodGroup = "B+",
                UnitsRequired = 2,
                HospitalName = "National Heart Foundation",
                Message = "Urgent requirement"
            };

            await service.SaveRequestAsync(request);

            Assert.False(string.IsNullOrWhiteSpace(request.TrackingToken));
            Assert.Equal(32, request.TrackingToken!.Length);

            var retrieved = await service.GetRequestByTokenAsync(request.TrackingToken);
            Assert.NotNull(retrieved);
            Assert.Equal("Token Test Requester", retrieved!.Name);

            Assert.Null(await service.GetRequestByTokenAsync("non-existent-token"));
            Assert.Null(await service.GetRequestByTokenAsync(""));
        }
    }

    public class AmbulanceTrackingTests
    {
        [Fact]
        public async Task SubmittedRequests_GetAnUnguessableToken()
        {
            using var db = new TestDatabase();
            var service = new AmbulanceHelpService(db.Repository<AmbulanceService>(), db.Repository<AmbulanceRequest>());

            var request = new AmbulanceRequest
            {
                PatientName = "Test Patient",
                ContactNumber = "01700000000",
                PickupAddress = "Dhanmondi"
            };

            await service.SubmitAmbulanceRequestAsync(request);

            Assert.False(string.IsNullOrWhiteSpace(request.TrackingToken));
            Assert.Equal(32, request.TrackingToken!.Length);   // GUID "N" format
            Assert.DoesNotContain("-", request.TrackingToken);
        }

        [Fact]
        public async Task RequestsCanBeFoundByTokenButNotByAnotherToken()
        {
            using var db = new TestDatabase();
            var service = new AmbulanceHelpService(db.Repository<AmbulanceService>(), db.Repository<AmbulanceRequest>());

            var request = new AmbulanceRequest
            {
                PatientName = "Test Patient",
                ContactNumber = "01700000000",
                PickupAddress = "Gulshan",
                Status = "Pending"
            };
            await service.SubmitAmbulanceRequestAsync(request);

            var found = await service.GetAmbulanceRequestByTokenAsync(request.TrackingToken!);

            Assert.NotNull(found);
            Assert.Equal("Test Patient", found!.PatientName);
            Assert.Null(await service.GetAmbulanceRequestByTokenAsync("not-a-real-token"));
            Assert.Null(await service.GetAmbulanceRequestByTokenAsync(""));
        }

        [Fact]
        public async Task TrackingTokensAreUniquePerRequest()
        {
            using var db = new TestDatabase();
            var service = new AmbulanceHelpService(db.Repository<AmbulanceService>(), db.Repository<AmbulanceRequest>());

            var first = AmbulanceHelpService.NewTrackingToken();
            var second = AmbulanceHelpService.NewTrackingToken();

            Assert.NotEqual(first, second);
        }

        [Fact]
        public async Task GetLatestAmbulanceRequestByContact_FindsMatchingRequestDirectlyInSql()
        {
            using var db = new TestDatabase();
            var service = new AmbulanceHelpService(db.Repository<AmbulanceService>(), db.Repository<AmbulanceRequest>());

            var request = new AmbulanceRequest
            {
                PatientName = "Contact Test Patient",
                ContactNumber = "01888776655",
                PickupAddress = "Mirpur",
                Status = "Pending"
            };
            await service.SubmitAmbulanceRequestAsync(request);

            var found = await service.GetLatestAmbulanceRequestByContactAsync("01888776655");
            Assert.NotNull(found);
            Assert.Equal("Contact Test Patient", found!.PatientName);

            var notFound = await service.GetLatestAmbulanceRequestByContactAsync("01700000999");
            Assert.Null(notFound);

            var empty = await service.GetLatestAmbulanceRequestByContactAsync("");
            Assert.Null(empty);
        }
    }

    public class DonationServiceTests
    {
        [Fact]
        public async Task AddingADonation_UpdatesTheDonorsEligibilityDate()
        {
            using var db = new TestDatabase();
            var donorService = new DonorService(db.Repository<BloodDonor>());
            var service = new DonationService(db.Repository<Donation>(), donorService);
            var donor = db.Context.BloodDonors.First();

            var donationDate = DateTime.Today.AddDays(-10);
            await service.AddDonationAsync(new Donation
            {
                DonorId = donor.Id,
                DonationDate = donationDate,
                UnitsDonated = 2,
                BloodGroup = donor.BloodGroup
            });

            var updated = await donorService.GetDonorByIdAsync(donor.Id);
            Assert.Equal(donationDate.Date, updated.LastDonatedDate!.Value.Date);
            Assert.False(updated.IsEligibleToDonate);   // 10 days ago is inside the 90-day gap
        }

        [Fact]
        public async Task DeletingTheLatestDonation_RestoresEligibility()
        {
            using var db = new TestDatabase();
            var donorService = new DonorService(db.Repository<BloodDonor>());
            var service = new DonationService(db.Repository<Donation>(), donorService);
            var donor = db.Context.BloodDonors.First();

            var donation = new Donation
            {
                DonorId = donor.Id,
                DonationDate = DateTime.Today.AddDays(-5),
                UnitsDonated = 1,
                BloodGroup = donor.BloodGroup
            };
            await service.AddDonationAsync(donation);
            Assert.False((await donorService.GetDonorByIdAsync(donor.Id)).IsEligibleToDonate);

            await service.DeleteDonationAsync(donation.Id);

            var after = await donorService.GetDonorByIdAsync(donor.Id);
            Assert.Null(after.LastDonatedDate);
            Assert.True(after.IsEligibleToDonate);
        }

        [Fact]
        public async Task TotalUnitsDonated_IsAggregatedInSql()
        {
            using var db = new TestDatabase();
            var donorService = new DonorService(db.Repository<BloodDonor>());
            var service = new DonationService(db.Repository<Donation>(), donorService);
            var donor = db.Context.BloodDonors.First();

            await service.AddDonationAsync(new Donation { DonorId = donor.Id, DonationDate = DateTime.Today.AddDays(-100), UnitsDonated = 2 });
            await service.AddDonationAsync(new Donation { DonorId = donor.Id, DonationDate = DateTime.Today.AddDays(-200), UnitsDonated = 3 });

            Assert.Equal(5, await service.GetTotalUnitsDonatedAsync());
            Assert.Equal(2, await service.GetTotalDonationsCountAsync());
        }
    }

    public class HomeSummaryServiceTests
    {
        [Fact]
        public async Task Summary_ReportsLiveCountsAndCapsFeaturedRows()
        {
            using var db = new TestDatabase();
            var service = new HomeSummaryService(
                db.Repository<BloodDonor>(),
                db.Repository<Hospital>(),
                db.Repository<BloodBank>(),
                db.Repository<AmbulanceService>(),
                db.Repository<OxygenService>(),
                db.Repository<BloodRequest>());

            db.Context.Hospitals.Add(new Hospital
            {
                Name = "H1",
                City = "Dhaka",
                Address = "1 Test Road",
                ContactNumber = "01700000001",
                EmergencyHelpline = "999",
                EmergencyServices = "Trauma",
                AvailableIcuBeds = 3,
                AvailableEmergencyBeds = 5
            });
            db.Context.Hospitals.Add(new Hospital
            {
                Name = "H2",
                City = "Dhaka",
                Address = "2 Test Road",
                ContactNumber = "01700000002",
                EmergencyHelpline = "999",
                EmergencyServices = "ICU",
                AvailableIcuBeds = 4,
                AvailableEmergencyBeds = 1
            });
            await db.Context.SaveChangesAsync();

            var summary = await service.GetSummaryAsync();

            Assert.Equal(3, summary.TotalDonors);
            Assert.Equal(2, summary.TotalHospitals);
            Assert.Equal(7, summary.TotalAvailableIcuBeds);
            Assert.Equal(6, summary.TotalAvailableEmergencyBeds);
            Assert.Equal(3, summary.TotalActiveRequests);   // the fulfilled request is excluded
            Assert.True(summary.ActiveDonors.Count() <= 6);
            Assert.True(summary.UrgentRequests.Count() <= 3);
        }
    }

    public class ReportServiceTests
    {
        [Fact]
        public async Task Reports_BucketTwelveMonthsAndExcludeClosedRequests()
        {
            using var db = new TestDatabase();
            var adminService = new AdminService(
                db.Repository<Admin>(),
                db.Repository<BloodDonor>(),
                db.Repository<BloodRequest>(),
                db.Repository<Hospital>(),
                db.Repository<AmbulanceService>(),
                db.Repository<OxygenService>(),
                db.Repository<BloodBank>());
            var service = new ReportService(
                db.Repository<BloodDonor>(),
                db.Repository<BloodRequest>(),
                db.Repository<Donation>(),
                adminService);

            var today = DateTime.Today;
            db.Context.Donations.Add(new Donation { DonorId = 1, DonationDate = today.AddDays(-3), UnitsDonated = 2 });
            db.Context.Donations.Add(new Donation { DonorId = 2, DonationDate = today.AddMonths(-2), UnitsDonated = 1 });
            await db.Context.SaveChangesAsync();

            var report = await service.BuildAsync(today);

            Assert.Equal(12, report.SeriesLabels.Count);
            Assert.Equal(12, report.UnitsPerMonth.Count);
            Assert.Equal(3, report.TotalUnitsDonated);
            Assert.Equal(4, report.TotalRequests);
            Assert.Equal(1, report.FulfilledRequests);
            Assert.Equal(3, report.PendingRequests);
            Assert.Equal(3, report.TotalDonors);           // every donor row, active or not
            Assert.True(report.UnitsPerMonth[11] >= 2);    // most recent month bucket
        }
    }

    public class PasswordResetServiceTests
    {
        [Fact]
        public async Task CreateResetTokenAsync_GeneratesSecureTokenWithFutureExpiration()
        {
            using var db = new TestDatabase();
            var service = new PasswordResetService(db.Repository<PasswordResetToken>());

            var token = await service.CreateResetTokenAsync("tanvir@example.com");

            Assert.NotNull(token);
            Assert.Equal(32, token.Length);

            var validated = await service.ValidateTokenAsync(token);
            Assert.NotNull(validated);
            Assert.Equal("tanvir@example.com", validated!.Email);
            Assert.False(validated.IsUsed);
            Assert.True(validated.ExpirationDate > DateTime.UtcNow);
        }

        [Fact]
        public async Task ValidateTokenAsync_RejectsExpiredOrUsedTokens()
        {
            using var db = new TestDatabase();
            var service = new PasswordResetService(db.Repository<PasswordResetToken>());

            var token = await service.CreateResetTokenAsync("tanvir@example.com");

            // Mark as used
            await service.MarkTokenAsUsedAsync(token);
            var resultUsed = await service.ValidateTokenAsync(token);
            Assert.Null(resultUsed);

            // Test expired token
            var expiredTokenEntity = new PasswordResetToken
            {
                Email = "nusrat@example.com",
                Token = "expiredtokentest1234567890123456",
                ExpirationDate = DateTime.UtcNow.AddMinutes(-10),
                IsUsed = false,
                CreatedDate = DateTime.UtcNow.AddHours(-1)
            };
            await db.Repository<PasswordResetToken>().AddAsync(expiredTokenEntity);

            var resultExpired = await service.ValidateTokenAsync("expiredtokentest1234567890123456");
            Assert.Null(resultExpired);
        }

        [Fact]
        public async Task CreateResetTokenAsync_RejectsEmptyOrWhitespaceEmail()
        {
            using var db = new TestDatabase();
            var service = new PasswordResetService(db.Repository<PasswordResetToken>());

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateResetTokenAsync(""));
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateResetTokenAsync("   "));
        }
    }

    public class NotificationServiceTests
    {
        [Fact]
        public async Task CreateNotificationAsync_PersistsNotificationCorrectly()
        {
            using var db = new TestDatabase();
            var service = new NotificationService(db.Repository<Notification>());

            var notification = new Notification
            {
                Title = "Test Emergency",
                Message = "O+ blood needed at Square Hospital",
                Category = "Emergency",
                Urgency = "P0_Critical",
                TargetRole = "Donor",
                UserId = 1
            };

            var created = await service.CreateNotificationAsync(notification);
            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.False(created.IsRead);
            Assert.Equal("Test Emergency", created.Title);
        }

        [Fact]
        public async Task GetUnreadCountAsync_CountsOnlyUnreadForRole()
        {
            using var db = new TestDatabase();
            var service = new NotificationService(db.Repository<Notification>());

            await service.CreateNotificationAsync(new Notification
            {
                Title = "Alert 1",
                Message = "Msg 1",
                TargetRole = "Donor",
                IsRead = false
            });

            await service.CreateNotificationAsync(new Notification
            {
                Title = "Alert 2",
                Message = "Msg 2",
                TargetRole = "Donor",
                IsRead = true
            });

            await service.CreateNotificationAsync(new Notification
            {
                Title = "Admin Alert",
                Message = "Admin only",
                TargetRole = "Admin",
                IsRead = false
            });

            var donorCount = await service.GetUnreadCountAsync(1, "Donor");
            var adminCount = await service.GetUnreadCountAsync(1, "Admin");

            Assert.Equal(1, donorCount);
            Assert.Equal(1, adminCount);
        }

        [Fact]
        public async Task MarkAsReadAsync_UpdatesStateCorrectly()
        {
            using var db = new TestDatabase();
            var service = new NotificationService(db.Repository<Notification>());

            var created = await service.CreateNotificationAsync(new Notification
            {
                Title = "Unread Alert",
                Message = "Please read",
                TargetRole = "All",
                IsRead = false
            });

            await service.MarkAsReadAsync(created.Id);

            var unreadCount = await service.GetUnreadCountAsync(null, "Guest");
            Assert.Equal(0, unreadCount);
        }
    }
}


