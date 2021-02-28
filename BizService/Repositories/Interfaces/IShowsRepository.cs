using System.Collections.Generic;
using System.Threading.Tasks;
using BizService.Models;

namespace BizService.Repositories
{
    public interface IShowsRepository
    {
        Task<Show> GetShowAsync(string id);
        Task<List<Show>> GetShowsAsync();
        Task AddShowAsync(Show show);
        Task UpdateShowAsync(Show show);
        Task<bool> IsExistAsync(string id);
    }
}