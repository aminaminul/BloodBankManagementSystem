using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class Admin
    {
        [Key]
        public int ID { get; set; }
        [StringLength(120)]
        public string AdminName { get; set; }
        [StringLength(120)]
        public string UserName { get; set; }
        public long? MobileNumber { get; set; }
        [StringLength(200)]
        public string Email { get; set; }
        [StringLength(200)]
        public string Password { get; set; }
        public DateTime? AdminRegdate { get; set; }
    }
}
