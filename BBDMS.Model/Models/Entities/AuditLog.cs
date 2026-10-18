using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    /// <summary>
    /// Append-only record of privileged actions (status changes, deletions, settings edits).
    /// Without it there was no way to answer "who changed this?" after the fact.
    /// </summary>
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        /// <summary>"Admin" or "Donor" — the role the actor was signed in with.</summary>
        [StringLength(20)]
        public string ActorRole { get; set; } = string.Empty;

        public int? ActorId { get; set; }

        [StringLength(120)]
        public string? ActorName { get; set; }

        /// <summary>Short verb, e.g. "StatusChanged", "Deleted", "SettingsUpdated".</summary>
        [StringLength(60)]
        public string Action { get; set; } = string.Empty;

        [StringLength(60)]
        public string EntityType { get; set; } = string.Empty;

        [StringLength(60)]
        public string? EntityId { get; set; }

        [StringLength(500)]
        public string? Details { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(45)]
        public string? IpAddress { get; set; }
    }
}
