using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BizService.Models;
using BizService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;

namespace BizService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IShowsRepository _showsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly int MAX_VOTES;

        public UsersController(IShowsRepository showsRepository, 
                               IUsersRepository usersRepository, 
                               IConfiguration config)
        {
            _showsRepository = showsRepository;
            _usersRepository = usersRepository;
            MAX_VOTES = config.GetValue<int>("MaxVotes");
        }

        [HttpPost("{phoneName}/vote")]
        public async Task<IActionResult> VoteAnonymouslyAsync(
            [FromRoute]
            [RegularExpression(@"^1[0-9]{10}$", ErrorMessage = "Invalid phone number format")]
            [Required]
            string phoneName,
            [FromBody]
            AnonymousVoteRequestDTO voteRequest)
        {
            if (!await _showsRepository.IsExistAsync(voteRequest.ShowId))
            {
                ModelState.AddModelError("ShowId", "Show is not exist");
                return BadRequest(ModelState);
            }

            var user = await _usersRepository.GetUserAsync(phoneName);
            if (user == null)
            {
                user = new User() { Id = phoneName };
            }

            if (user.VotesCount >= MAX_VOTES)
            {
                ModelState.AddModelError("PhoneName", "Your number of votes has run out");
                return BadRequest(ModelState);
            }

            user.Votes.Add(voteRequest.ShowId);
            await _usersRepository.SaveUserAsync(user);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAsync(string id)
        {
            if (!await _usersRepository.IsExistAsync(id))
            {
                return NotFound();
            }
            var user = await _usersRepository.GetUserAsync(id);
            return Ok(user);
        }
    }
}