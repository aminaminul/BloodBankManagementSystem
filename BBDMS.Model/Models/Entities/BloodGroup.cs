using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class BloodGroup
    {
        [Key]
        public int Id { get; set; }
        [StringLength(20)]
        public string GroupName { get; set; }
        public DateTime PostingDate { get; set; } = DateTime.Now;
    }
}
