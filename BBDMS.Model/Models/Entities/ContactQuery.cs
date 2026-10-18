using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class ContactQuery
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(120)]
        public string EmailId { get; set; } = string.Empty;
        [StringLength(11)]
        public string ContactNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime PostingDate { get; set; } = DateTime.Now;
        public int? Status { get; set; }
    }
}
