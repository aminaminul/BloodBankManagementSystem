using BBDMS.Model.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BBDMS.Repository.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<BloodDonor> BloodDonors { get; set; }
        public DbSet<BloodGroup> BloodGroups { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<ContactQuery> ContactQueries { get; set; }
        public DbSet<PageContent> PageContents { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<BloodBank> BloodBanks { get; set; }
        public DbSet<AmbulanceService> AmbulanceServices { get; set; }
        public DbSet<AmbulanceRequest> AmbulanceRequests { get; set; }
        public DbSet<OxygenService> OxygenServices { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Table Mappings
            modelBuilder.Entity<Admin>().ToTable("tbladmin");
            modelBuilder.Entity<BloodDonor>().ToTable("tblblooddonars");
            modelBuilder.Entity<BloodGroup>().ToTable("tblbloodgroup");
            modelBuilder.Entity<BloodRequest>().ToTable("tblbloodrequirer");
            modelBuilder.Entity<ContactInfo>().ToTable("tblcontactusinfo");
            modelBuilder.Entity<ContactQuery>().ToTable("tblcontactusquery");
            modelBuilder.Entity<PageContent>().ToTable("tblpages");
            modelBuilder.Entity<Hospital>().ToTable("tblhospitals");
            modelBuilder.Entity<BloodBank>().ToTable("tblbloodbanks");
            modelBuilder.Entity<AmbulanceService>().ToTable("tblambulances");
            modelBuilder.Entity<AmbulanceRequest>().ToTable("tblambulancerequests");
            modelBuilder.Entity<OxygenService>().ToTable("tbloxygenservices");
            modelBuilder.Entity<Donation>().ToTable("tbldonationhistory");
            modelBuilder.Entity<AuditLog>().ToTable("tblauditlog");
            modelBuilder.Entity<PasswordResetToken>().ToTable("tblpasswordresettokens");
            modelBuilder.Entity<Notification>().ToTable("tblnotifications");

            // Unique Indexes
            modelBuilder.Entity<BloodDonor>()
                .HasIndex(d => d.EmailId)
                .IsUnique();

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.UserName)
                .IsUnique();

            modelBuilder.Entity<AmbulanceRequest>()
                .HasIndex(r => r.TrackingToken)
                .IsUnique()
                .HasFilter("[TrackingToken] IS NOT NULL");

            modelBuilder.Entity<BloodRequest>()
                .HasIndex(r => r.TrackingToken)
                .IsUnique()
                .HasFilter("[TrackingToken] IS NOT NULL");

            // Foreign Key Relationships
            modelBuilder.Entity<BloodRequest>()
                .HasOne<BloodDonor>()
                .WithMany()
                .HasForeignKey(r => r.BloodDonorID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Donation>()
                .HasOne<BloodDonor>()
                .WithMany()
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AmbulanceRequest>()
                .HasOne<AmbulanceService>()
                .WithMany()
                .HasForeignKey(r => r.AmbulanceServiceId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
