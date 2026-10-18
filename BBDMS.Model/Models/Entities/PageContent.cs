using System;
using System.ComponentModel.DataAnnotations;

namespace BBDMS.Model.Models.Entities
{
    public class PageContent
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string PageName { get; set; } = string.Empty;
        [StringLength(255)]
        public string Type { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }
}
