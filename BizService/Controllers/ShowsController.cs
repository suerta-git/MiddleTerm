using System.Collections.Generic;
using System.Threading.Tasks;
using BizService.Models;
using BizService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BizService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShowsController : ControllerBase
    {
        private IShowsRepository _showsRepository;
        public ShowsController(IShowsRepository showsRepository)
        {
            _showsRepository = showsRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShowAsync([FromRoute] string id)
        {
            var show = await _showsRepository.GetShowAsync(id);
            if (show == null)
            {
                return NotFound();
            }
            return Ok(show);
        }

        [HttpGet]
        public async Task<List<Show>> GetShowsAsync()
        {
            return await _showsRepository.GetShowsAsync();
        }

        [HttpPost]
        public async Task<IActionResult> AddShowAsync([FromBody][Bind("Name", "Type", "Performers")] Show show)
        {
            await _showsRepository.AddShowAsync(show);
            return CreatedAtAction("GetShow", new { id = show.Id }, show);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShowAsync([FromRoute] string id,
                                                         [FromBody][Bind("Name", "Type", "Performers")] Show show)
        {
            if (!await _showsRepository.IsExistAsync(id))
            {
                return NotFound();
            }
            show.Id = id;
            await _showsRepository.UpdateShowAsync(show);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShowAsync([FromRoute] string id)
        {
            if (!await _showsRepository.IsExistAsync(id))
            {
                return NotFound();
            }
            await _showsRepository.DeleteShowAsync(id);
            return NoContent();
        }
    }
}