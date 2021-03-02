using System.ComponentModel.DataAnnotations;

namespace BFFService.Models
{
    public class AnonymousVoteRequestDTO
    {
        [MinLength(1)]
        [Required]
        public string ShowId { get; set;}
    }
}