using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BFFService.HttpClients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BFFService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly ShowsClient _client;

        public ShowsController(ShowsClient client)
        {
            _client = client;
        }

        [HttpGet]
        public async Task<Stream> GetShowsAsync()
        {
            var response = await _client.GetShowsAsync();
            copyStatusAndHeaders(response, Response);
            return await response.Content.ReadAsStreamAsync();
        }

        [HttpGet("{id}")]
        public async Task<Stream> GetShowAsync([FromRoute] string id)
        {
            var response = await _client.GetShowAsync(id);
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