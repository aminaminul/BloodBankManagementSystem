using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BBDMS.Model.Models.Entities
{
    [Table("tblnotifications")]
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Target user identifier, or null/0 if intended for an entire role or broadcast.</summary>
        public int? UserId { get; set; }

        /// <summary>Target role: "Donor", "Admin", or "All".</summary>
        [StringLength(50)]
        public string TargetRole { get; set; } = "All";

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        /// <summary>Category: "Emergency", "Status", "Milestone", "System".</summary>
        [StringLength(50)]
        public string Category { get; set; } = "Emergency";

        /// <summary>Urgency tier: "P0_Critical", "P1_Operational", "P2_Informational".</summary>
        [StringLength(50)]
        public string Urgency { get; set; } = "P1_Operational";

        /// <summary>Direct URL to open when clicking the notification.</summary>
        [StringLength(500)]
        public string? TargetUrl { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
