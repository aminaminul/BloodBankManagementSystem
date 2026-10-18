using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    /// <summary>
    /// A single blood donation recorded by a donor. This is the source of truth
    /// for a donor's donation history, eligibility gap and reporting.
    /// </summary>
    public class Donation
    {
        [Key]
        public int Id { get; set; }

        public int DonorId { get; set; }

        [Required(ErrorMessage = "Donation date is required.")]
        [DataType(DataType.Date)]
        public DateTime DonationDate { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string? BloodGroup { get; set; }

        /// <summary>Hospital, blood bank or donation camp where the donation took place.</summary>
        [StringLength(250)]
        public string? CenterName { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [Range(1, 5, ErrorMessage = "Units donated must be between 1 and 5.")]
        public int UnitsDonated { get; set; } = 1;

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
