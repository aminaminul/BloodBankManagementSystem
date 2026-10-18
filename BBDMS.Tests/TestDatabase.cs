using System;
using BBDMS.Model.Models.Entities;
using BBDMS.Repository.Data;
using BBDMS.Repository.Interfaces;
using BBDMS.Repository.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BBDMS.Tests
{
    /// <summary>
    /// A real relational database (SQLite in-memory) for tests: queries, paging, GROUP BY and
    /// constraints behave like they do in production instead of being faked by a mock.
    /// The connection is kept open for the lifetime of the fixture, otherwise SQLite drops the schema.
    /// </summary>
    public sealed class TestDatabase : IDisposable
    {
        private readonly SqliteConnection _connection;

        public ApplicationDbContext Context { get; }

        public TestDatabase(bool seed = true)
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            Context = new ApplicationDbContext(options);
            Context.Database.EnsureCreated();

            if (seed)
            {
                Seed();
            }
        }

        public IRepository<T> Repository<T>() where T : class => new Repository<T>(Context);

        private void Seed()
        {
            Context.BloodGroups.AddRange(
                new BloodGroup { GroupName = "O+", PostingDate = DateTime.Now },
                new BloodGroup { GroupName = "A-", PostingDate = DateTime.Now });

            Context.BloodDonors.AddRange(
                new BloodDonor
                {
                    FullName = "Tanvir Ahmed",
                    MobileNumber = "01712341234",
                    EmailId = "tanvir@example.com",
                    Gender = "Male",
                    Age = 28,
                    BloodGroup = "O+",
                    Address = "Dhanmondi, Dhaka",
                    Status = 1,
                    IsAvailable = true,
                    Password = "$2b$11$notarealhashnotarealhashnotarealhashnotarealhashnota"
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
                    Status = 1,
                    IsAvailable = true,
                    Password = "$2b$11$notarealhashnotarealhashnotarealhashnotarealhashnota"
                },
                new BloodDonor
                {
                    FullName = "Disabled Donor",
                    MobileNumber = "01912341234",
                    EmailId = "disabled@example.com",
                    Gender = "Male",
                    Age = 40,
                    BloodGroup = "O+",
                    Address = "Dhaka",
                    Status = 0,
                    Password = "$2b$11$notarealhashnotarealhashnotarealhashnotarealhashnota"
                });

            Context.SaveChanges();

            var donor = Context.BloodDonors.First();

            Context.BloodRequests.AddRange(
                new BloodRequest
                {
                    Name = "Open O+ request",
                    EmailId = "open@example.com",
                    ContactNumber = "01711223344",
                    BloodRequireFor = "Surgery",
                    BloodGroup = "O+",
                    UnitsRequired = 2,
                    HospitalName = "City Hospital",
                    Location = "Dhaka",
                    Urgency = "Critical",
                    Status = "Pending",
                    Message = "Open community request",
                    ApplyDate = DateTime.Now.AddHours(-2)
                },
                new BloodRequest
                {
                    Name = "Already assigned",
                    EmailId = "assigned@example.com",
                    ContactNumber = "01711223355",
                    BloodRequireFor = "Delivery",
                    BloodGroup = "O+",
                    UnitsRequired = 1,
                    Location = "Dhaka",
                    Status = "Pending",
                    Message = "Assigned to a donor",
                    BloodDonorID = donor.Id,
                    ApplyDate = DateTime.Now.AddHours(-1)
                },
                new BloodRequest
                {
                    Name = "Closed request",
                    EmailId = "closed@example.com",
                    ContactNumber = "01711223366",
                    BloodRequireFor = "Accident",
                    BloodGroup = "O+",
                    UnitsRequired = 1,
                    Location = "Dhaka",
                    Status = "Fulfilled",
                    Message = "Already fulfilled",
                    ApplyDate = DateTime.Now.AddHours(-3)
                },
                new BloodRequest
                {
                    Name = "A- request",
                    EmailId = "aneg@example.com",
                    ContactNumber = "01711223377",
                    BloodRequireFor = "Thalassemia",
                    BloodGroup = "A-",
                    UnitsRequired = 1,
                    Location = "Sylhet",
                    Status = "Pending",
                    Message = "Rare group",
                    ApplyDate = DateTime.Now.AddMinutes(-30)
                });

            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
            _connection.Dispose();
        }
    }
}
