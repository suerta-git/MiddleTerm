using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using BizService.Models;
using BizService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BizService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VotesController : ControllerBase
    {
        private readonly IShowsRepository _showsRepository;
        private readonly IUsersRepository _usersRepository;

        private readonly IDynamoDBContext _context;
        private readonly int MAX_VOTES;

        public VotesController(IShowsRepository showsRepository,
                               IUsersRepository usersRepository,
                               IConfiguration config,
                               IDynamoDBContext context)
        {
            _showsRepository = showsRepository;
            _usersRepository = usersRepository;
            MAX_VOTES = config.GetValue<int>("MaxVotes");
            _context = context;
        }

        [HttpPost("{phoneName}")]
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

        [HttpPost("Count")]
        public async Task<IActionResult> CountVotesAsync()
        {
            var table = _context.GetTargetTable<User>();
            var voteResult = await table.GetItemAsync("VoteResult");
            if (voteResult != null)
            {
                return NoContent();
            }

            var showIdToPhoneNumbers = (await _showsRepository.GetShowsAsync())
                .ToDictionary(show => show.Id, ignore => new List<string>());

            var users = await _usersRepository.GetUsersAsync();
            int totalVotes = 0;
            foreach (var user in users)
            {
                foreach (var showId in user.Votes)
                {
                    showIdToPhoneNumbers[showId].Add(user.Id);
                    totalVotes++;
                }
            }

            if (totalVotes == 0)
            {
                return BadRequest("No votes");
            }

            var result = showIdToPhoneNumbers.Select(pair => 
                {
                    var doc = new Document();
                    doc["showId"] = pair.Key;
                    doc["votesCount"] = pair.Value.Count;
                    doc["votes"] = pair.Value;
                    doc["voteRate"] = (pair.Value.Count / (decimal)totalVotes).ToString("P");
                    return doc;
                })
                .OrderByDescending(show => show["votesCount"].AsInt())
                .ToList();

            var doc = new Document();
            doc["Id"] = "VoteResult";
            doc["Result"] = result;
            await table.PutItemAsync(doc);

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetVoteResultAsync()
        {
            var table = _context.GetTargetTable<User>();
            var voteResult = await table.GetItemAsync("VoteResult");
            if (voteResult == null)
            {
                return NotFound();
            }
            return Content(voteResult.ToJson(), "application/json");
        }

        [HttpPost("Draw")]
        public async Task<IActionResult> DrawPrizesAsync(
            [Range(1, int.MaxValue)]
            [Required]
            int totalPrizes,
            [Range(1, int.MaxValue)]
            [Required]
            int fromTopN)
        {
            var table = _context.GetTargetTable<User>();
            var voteResult = await table.GetItemAsync("VoteResult");
            if (voteResult == null)
            {
                return NotFound();
            }

            var phoneNumbers = voteResult["Result"].AsListOfDocument()
                .Take(fromTopN)
                .SelectMany(show => show["votes"].AsListOfString())
                .ToList();

            var prizes = new List<string>();
            while (prizes.Count < totalPrizes && phoneNumbers.Count > 0)
            {
                var index = new Random().Next(phoneNumbers.Count);
                prizes.Add(phoneNumbers[index]);
                phoneNumbers.RemoveAt(index);
            }
            return Ok(prizes);
        }
    }
}