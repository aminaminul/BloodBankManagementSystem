using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class ContactQuery
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(120)]
        public string EmailId { get; set; }
        [StringLength(11)]
        public string ContactNumber { get; set; }
        public string Message { get; set; }
        public DateTime PostingDate { get; set; } = DateTime.Now;
        public int? Status { get; set; }
    }
}
