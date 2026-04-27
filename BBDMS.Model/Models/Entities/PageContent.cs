using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class PageContent
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string PageName { get; set; }
        [StringLength(255)]
        public string Type { get; set; }
        public string Detail { get; set; }
    }
}
