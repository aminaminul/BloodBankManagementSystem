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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Map table names to match PHP names if preferred, or keep standard
            modelBuilder.Entity<Admin>().ToTable("tbladmin");
            modelBuilder.Entity<BloodDonor>().ToTable("tblblooddonars");
            modelBuilder.Entity<BloodGroup>().ToTable("tblbloodgroup");
            modelBuilder.Entity<BloodRequest>().ToTable("tblbloodrequirer");
            modelBuilder.Entity<ContactInfo>().ToTable("tblcontactusinfo");
            modelBuilder.Entity<ContactQuery>().ToTable("tblcontactusquery");
            modelBuilder.Entity<PageContent>().ToTable("tblpages");
        }
    }
}
