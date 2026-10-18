using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class Admin
    {
        [Key]
        public int ID { get; set; }
        [StringLength(120)]
        public string AdminName { get; set; } = string.Empty;
        [StringLength(120)]
        public string UserName { get; set; } = string.Empty;
        /// <summary>Text, not a number: leading zeros and +8801… prefixes must survive.</summary>
        [StringLength(20)]
        [RegularExpression(@"^[0-9+\-\s()]{7,20}$", ErrorMessage = "Please enter a valid phone number.")]
        public string? MobileNumber { get; set; }
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;
        [StringLength(200)]
        public string Password { get; set; } = string.Empty;
        public DateTime? AdminRegdate { get; set; }

        /// <summary>Preferred UI theme for this administrator ("light" or "dark").</summary>
        [StringLength(10)]
        public string? ThemePreference { get; set; }
    }
}
