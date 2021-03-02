using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BFFService.Models
{
    public class User
    {
        [RegularExpression(@"^1[0-9]{10}$", ErrorMessage = "Invalid phone number format")]
        [Required]
        public string Id { get; set; }
        public List<string> Votes { get; set; } = new List<string>();

        public int VotesCount => Votes.Count;
    }
}