using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BFFService.Models
{
    public class Show
    {
        public string Id { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public List<string> Performers { get; set; } = new List<string>();
    }
}