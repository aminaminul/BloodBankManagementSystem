using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class ContactInfo
    {
        [Key]
        public int Id { get; set; }
        public string Address { get; set; }
        [StringLength(255)]
        public string EmailId { get; set; }
        [StringLength(11)]
        public string ContactNo { get; set; }
    }
}
