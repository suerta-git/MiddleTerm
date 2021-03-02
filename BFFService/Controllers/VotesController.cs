using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BFFService.HttpClients;
using BFFService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BFFService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VotesController : ControllerBase
    {
        private readonly VotesClient _client;

        public VotesController(VotesClient client)
        {
            _client = client;
        }

        [HttpPost("{phoneName}")]
        public async Task<Stream> VoteAnonymouslyAsync(
            [FromRoute]
            [RegularExpression(@"^1[0-9]{10}$", ErrorMessage = "Invalid phone number format")]
            [Required]
            string phoneName,
            [FromBody]
            AnonymousVoteRequestDTO voteRequest)
        {
            var response = await _client.VoteAnonymouslyAsync(phoneName, voteRequest);
            copyStatusAndHeaders(response, Response);
            return await response.Content.ReadAsStreamAsync();
        }

        [HttpGet("{id}")]
        public async Task<Stream> GetUserAsync([FromRoute] string id)
        {
            var response = await _client.GetUserAsync(id);
            copyStatusAndHeaders(response, Response);
            return await response.Content.ReadAsStreamAsync();
        }

        [HttpPost("Count")]
        public async Task<Stream> CountVotesAsync()
        {
            var response = await _client.CountVotesAsync();
            copyStatusAndHeaders(response, Response);
            return await response.Content.ReadAsStreamAsync();
        }

        [HttpGet]
        public async Task<Stream> GetVoteResultAsync()
        {
            var response = await _client.GetVoteResultAsync();
            copyStatusAndHeaders(response, Response);
            return await response.Content.ReadAsStreamAsync();
        }
        
        [HttpPost("Draw")]
        public async Task<Stream> DrawPrizesAsync(
            [Range(1, int.MaxValue)]
            [Required]
            int totalPrizes,
            [Range(1, int.MaxValue)]
            [Required]
            int fromTopN)
        {
            var response = await _client.DrawPrizesAsync(totalPrizes, fromTopN);
            copyStatusAndHeaders(response, Response);
            return await response.Content.ReadAsStreamAsync();
        }

        private void copyStatusAndHeaders(HttpResponseMessage response, HttpResponse finalResponse)
        {
            finalResponse.StatusCode = (int)response.StatusCode;
            foreach (var header in response.Content.Headers)
            {
                finalResponse.Headers[header.Key] = header.Value.ToArray();
            }
        }
    }
}