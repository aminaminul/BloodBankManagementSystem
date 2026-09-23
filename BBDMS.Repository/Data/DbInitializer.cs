using System;
using System.Linq;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Repository.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Seed Admin if not exists
            if (!context.Admins.Any())
            {
                await context.Admins.AddAsync(new Admin
                {
                    AdminName = "System Administrator",
                    UserName = "admin",
                    MobileNumber = 1234567890,
                    Email = "admin@emergencyhelp.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin", 11),
                    AdminRegdate = DateTime.Now
                });
            }
            else
            {
                // Auto-upgrade existing admin password if in plain text
                var admin = context.Admins.FirstOrDefault();
                if (admin != null && !string.IsNullOrEmpty(admin.Password) && !admin.Password.StartsWith("$2"))
                {
                    admin.Password = BCrypt.Net.BCrypt.HashPassword(admin.Password, 11);
                    context.Admins.Update(admin);
                }
            }

            // Seed Blood Groups if not exists
            if (!context.BloodGroups.Any())
            {
                var groups = new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
                foreach (var g in groups)
                {
                    await context.BloodGroups.AddAsync(new BloodGroup
                    {
                        GroupName = g,
                        PostingDate = DateTime.Now
                    });
                }
            }

            // Seed Contact Info if not exists
            if (!context.ContactInfos.Any())
            {
                await context.ContactInfos.AddAsync(new ContactInfo
                {
                    Address = "Central Emergency Support Tower, Medical District",
                    EmailId = "emergency@bbdms-support.org",
                    ContactNo = "01700000000"
                });
            }

            // Seed Page Contents if not exists
            if (!context.PageContents.Any())
            {
                await context.PageContents.AddAsync(new PageContent
                {
                    PageName = "About Us",
                    Type = "aboutus",
                    Detail = "The Blood Donor and Emergency Help System is a dedicated web-based platform committed to saving lives. We connect voluntary blood donors, emergency patients, hospitals with available ICU and emergency beds, ambulance fleets, and medical oxygen suppliers all under one roof."
                });
                await context.PageContents.AddAsync(new PageContent
                {
                    PageName = "Donor Information",
                    Type = "donor",
                    Detail = "Blood donation is one of the most noble acts of kindness. A single donation can save up to three lives. Check your eligibility and register as a lifesaver today."
                });
            }

            // Seed Hospitals if not exists
            if (!context.Hospitals.Any())
            {
                await context.Hospitals.AddRangeAsync(
                    new Hospital
                    {
                        Name = "City Central General Hospital",
                        Address = "12 Healthcare Avenue, Downtown",
                        City = "Dhaka",
                        ContactNumber = "01711122233",
                        EmergencyHelpline = "999 / 01711122299",
                        EmergencyServices = "24/7 Trauma Care, Stroke Center, Burn Unit, Blood Transfusion",
                        TotalEmergencyBeds = 40,
                        AvailableEmergencyBeds = 14,
                        TotalIcuBeds = 20,
                        AvailableIcuBeds = 5,
                        LastUpdated = DateTime.Now
                    },
                    new Hospital
                    {
                        Name = "Metropolitan Medical College & Hospital",
                        Address = "45 Medical College Road",
                        City = "Dhaka",
                        ContactNumber = "01822233344",
                        EmergencyHelpline = "01822233399",
                        EmergencyServices = "Emergency Surgery, Cardiac Care, Dialysis, Pediatric ICU",
                        TotalEmergencyBeds = 60,
                        AvailableEmergencyBeds = 22,
                        TotalIcuBeds = 25,
                        AvailableIcuBeds = 8,
                        LastUpdated = DateTime.Now
                    },
                    new Hospital
                    {
                        Name = "Apollo Crescent Specialized Hospital",
                        Address = "88 West Coast Road",
                        City = "Chittagong",
                        ContactNumber = "01933344455",
                        EmergencyHelpline = "01933344499",
                        EmergencyServices = "24/7 Critical Care, Neurosurgery, Emergency Resuscitation",
                        TotalEmergencyBeds = 30,
                        AvailableEmergencyBeds = 9,
                        TotalIcuBeds = 15,
                        AvailableIcuBeds = 3,
                        LastUpdated = DateTime.Now
                    },
                    new Hospital
                    {
                        Name = "Green Life Emergency Hospital",
                        Address = "104 University Circle",
                        City = "Sylhet",
                        ContactNumber = "01644455566",
                        EmergencyHelpline = "01644455599",
                        EmergencyServices = "24-Hour Emergency, Oxygen Support, Neonatal ICU",
                        TotalEmergencyBeds = 25,
                        AvailableEmergencyBeds = 6,
                        TotalIcuBeds = 10,
                        AvailableIcuBeds = 2,
                        LastUpdated = DateTime.Now
                    }
                );
            }

            // Seed Blood Banks if not exists
            if (!context.BloodBanks.Any())
            {
                await context.BloodBanks.AddRangeAsync(
                    new BloodBank
                    {
                        Name = "Red Crescent Central Blood Bank",
                        HospitalAffiliation = "Bangladesh Red Crescent Society",
                        Address = "684 Red Cross Road",
                        City = "Dhaka",
                        ContactNumber = "01755566677",
                        OperatingHours = "24 Hours (7 Days)",
                        AvailableBloodGroups = "A+, A-, B+, B-, O+, O-, AB+, AB-",
                        LastUpdated = DateTime.Now
                    },
                    new BloodBank
                    {
                        Name = "National Blood Transfusion Center",
                        HospitalAffiliation = "City Central Hospital",
                        Address = "12 Healthcare Avenue",
                        City = "Dhaka",
                        ContactNumber = "01866677788",
                        OperatingHours = "24 Hours",
                        AvailableBloodGroups = "A+, B+, O+, AB+, O-",
                        LastUpdated = DateTime.Now
                    },
                    new BloodBank
                    {
                        Name = "LifeLine Blood Center & Bank",
                        HospitalAffiliation = "Apollo Crescent Hospital",
                        Address = "88 West Coast Road",
                        City = "Chittagong",
                        ContactNumber = "01977788899",
                        OperatingHours = "24 Hours",
                        AvailableBloodGroups = "A+, B+, O+, AB+",
                        LastUpdated = DateTime.Now
                    }
                );
            }

            // Seed Ambulance Services if not exists
            if (!context.AmbulanceServices.Any())
            {
                await context.AmbulanceServices.AddRangeAsync(
                    new AmbulanceService
                    {
                        ProviderName = "SwiftCare ICU Ambulance Fleet",
                        Location = "Dhaka",
                        AmbulanceType = "Advanced Life Support (ALS) / ICU",
                        VehicleNumber = "DHK-METRO-1122",
                        ContactNumber = "01788899900",
                        IsAvailable = true,
                        ServiceHours = "24/7",
                        LastUpdated = DateTime.Now
                    },
                    new AmbulanceService
                    {
                        ProviderName = "RapidAid Medical Transport",
                        Location = "Dhaka",
                        AmbulanceType = "Basic Life Support (BLS)",
                        VehicleNumber = "DHK-METRO-3344",
                        ContactNumber = "01899900011",
                        IsAvailable = true,
                        ServiceHours = "24/7",
                        LastUpdated = DateTime.Now
                    },
                    new AmbulanceService
                    {
                        ProviderName = "Coastal Emergency Ambulance",
                        Location = "Chittagong",
                        AmbulanceType = "Advanced Life Support (ALS) / ICU",
                        VehicleNumber = "CTG-METRO-5566",
                        ContactNumber = "01900011122",
                        IsAvailable = true,
                        ServiceHours = "24/7",
                        LastUpdated = DateTime.Now
                    },
                    new AmbulanceService
                    {
                        ProviderName = "LifeSaver Neonatal & AC Ambulance",
                        Location = "Sylhet",
                        AmbulanceType = "AC Ambulance (Neonatal Support)",
                        VehicleNumber = "SYL-METRO-7788",
                        ContactNumber = "01611122233",
                        IsAvailable = true,
                        ServiceHours = "24/7",
                        LastUpdated = DateTime.Now
                    }
                );
            }

            // Seed Oxygen Services if not exists
            if (!context.OxygenServices.Any())
            {
                await context.OxygenServices.AddRangeAsync(
                    new OxygenService
                    {
                        ProviderName = "OxyPure Medical Gas & Refill",
                        Location = "Dhaka",
                        Address = "Plot 24, Industrial Zone, Tejgaon",
                        ContactNumber = "01712345678",
                        OxygenAvailability = "In Stock",
                        CylinderCapacity = "1.4m³, 6.8m³, 10L Concentrators",
                        HomeDeliveryAvailable = true,
                        ServiceDetails = "24/7 emergency delivery of medical oxygen cylinders with flowmeter, mask & regulator.",
                        LastUpdated = DateTime.Now
                    },
                    new OxygenService
                    {
                        ProviderName = "BreatheEasy Oxygen Supplies",
                        Location = "Chittagong",
                        Address = "15 Port Connecting Road",
                        ContactNumber = "01812345678",
                        OxygenAvailability = "In Stock",
                        CylinderCapacity = "1.4m³, 6.8m³, Portable Canisters",
                        HomeDeliveryAvailable = true,
                        ServiceDetails = "Prompt delivery across city, cylinder refills, pulse oximeters, and hospital setup support.",
                        LastUpdated = DateTime.Now
                    },
                    new OxygenService
                    {
                        ProviderName = "QuickCare Oxygen Hub",
                        Location = "Sylhet",
                        Address = "5 Zindabazar Center",
                        ContactNumber = "01912345678",
                        OxygenAvailability = "In Stock",
                        CylinderCapacity = "Medium (6.8m³), Jumbo Cylinders",
                        HomeDeliveryAvailable = false,
                        ServiceDetails = "Walk-in emergency refills and certified medical grade oxygen cylinder sales.",
                        LastUpdated = DateTime.Now
                    }
                );
            }

            // Seed Sample Donors if none exist
            if (!context.BloodDonors.Any())
            {
                await context.BloodDonors.AddRangeAsync(
                    new BloodDonor
                    {
                        FullName = "Tanvir Ahmed",
                        MobileNumber = "01712341234",
                        EmailId = "tanvir@example.com",
                        Gender = "Male",
                        Age = 28,
                        BloodGroup = "O+",
                        Address = "Dhanmondi, Dhaka",
                        Message = "Ready to donate anytime for emergencies.",
                        PostingDate = DateTime.Now,
                        Status = 1,
                        IsAvailable = true,
                        Password = BCrypt.Net.BCrypt.HashPassword("donor", 11)
                    },
                    new BloodDonor
                    {
                        FullName = "Nusrat Jahan",
                        MobileNumber = "01812341234",
                        EmailId = "nusrat@example.com",
                        Gender = "Female",
                        Age = 25,
                        BloodGroup = "A+",
                        Address = "Gulshan, Dhaka",
                        Message = "Available on weekends and urgent requests.",
                        PostingDate = DateTime.Now,
                        Status = 1,
                        IsAvailable = true,
                        Password = BCrypt.Net.BCrypt.HashPassword("donor", 11)
                    },
                    new BloodDonor
                    {
                        FullName = "Rahim Chowdhury",
                        MobileNumber = "01912341234",
                        EmailId = "rahim@example.com",
                        Gender = "Male",
                        Age = 32,
                        BloodGroup = "B+",
                        Address = "Agrabad, Chittagong",
                        Message = "Regular donor willing to travel within Chittagong.",
                        PostingDate = DateTime.Now,
                        Status = 1,
                        IsAvailable = true,
                        Password = BCrypt.Net.BCrypt.HashPassword("donor", 11)
                    },
                    new BloodDonor
                    {
                        FullName = "Sadia Islam",
                        MobileNumber = "01612341234",
                        EmailId = "sadia@example.com",
                        Gender = "Female",
                        Age = 24,
                        BloodGroup = "AB-",
                        Address = "Zindabazar, Sylhet",
                        Message = "Rare blood group donor, contact for critical cases.",
                        PostingDate = DateTime.Now,
                        Status = 1,
                        IsAvailable = true,
                        Password = BCrypt.Net.BCrypt.HashPassword("donor", 11)
                    }
                );
            }
            else
            {
                // Auto-upgrade existing plain-text donor passwords
                var donors = context.BloodDonors.ToList();
                foreach (var d in donors)
                {
                    if (!string.IsNullOrEmpty(d.Password) && !d.Password.StartsWith("$2"))
                    {
                        d.Password = BCrypt.Net.BCrypt.HashPassword(d.Password, 11);
                        context.BloodDonors.Update(d);
                    }
                }
            }

            // Seed Sample Emergency Requests if none exist
            if (!context.BloodRequests.Any())
            {
                await context.BloodRequests.AddRangeAsync(
                    new BloodRequest
                    {
                        Name = "Farhan Rahman",
                        EmailId = "farhan@example.com",
                        ContactNumber = 1711223344,
                        BloodGroup = "O+",
                        BloodRequireFor = "Major Heart Surgery",
                        UnitsRequired = 3,
                        HospitalName = "City Central General Hospital",
                        Location = "Dhaka",
                        Urgency = "Critical",
                        Status = "Pending",
                        Message = "Patient admitted in ICU Bed 4. Requires 3 units of O+ blood by tomorrow morning.",
                        ApplyDate = DateTime.Now
                    },
                    new BloodRequest
                    {
                        Name = "Shamim Hossain",
                        EmailId = "shamim@example.com",
                        ContactNumber = 1822334455,
                        BloodGroup = "A-",
                        BloodRequireFor = "Accident Emergency",
                        UnitsRequired = 2,
                        HospitalName = "Metropolitan Medical College & Hospital",
                        Location = "Dhaka",
                        Urgency = "Critical",
                        Status = "Pending",
                        Message = "Accident trauma patient in emergency ward. Urgent A- negative blood needed immediately.",
                        ApplyDate = DateTime.Now
                    }
                );
            }

            await context.SaveChangesAsync();
        }
    }
}
