using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class BloodRequest
    {
        [Key]
        public int ID { get; set; }
        public int? BloodDonorID { get; set; }
        [StringLength(250)]
        public string Name { get; set; }
        [StringLength(250)]
        public string EmailId { get; set; }
        public long? ContactNumber { get; set; }
        [StringLength(250)]
        public string BloodRequireFor { get; set; }
        public string Message { get; set; }
        public DateTime? ApplyDate { get; set; } = DateTime.Now;
    }
}
