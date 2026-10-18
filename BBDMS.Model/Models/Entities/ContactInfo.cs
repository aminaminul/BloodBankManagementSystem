using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class ContactInfo
    {
        [Key]
        public int Id { get; set; }
        public string Address { get; set; } = string.Empty;
        [StringLength(255)]
        public string EmailId { get; set; } = string.Empty;
        [StringLength(11)]
        public string ContactNo { get; set; } = string.Empty;
    }
}
