using System.ComponentModel.DataAnnotations;

namespace BizService.Models
{
    public class AnonymousVoteRequestDTO
    {
        [MinLength(1)]
        [Required]
        public string ShowId { get; set;}
    }
}