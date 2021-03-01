using System.Collections.Generic;
using System.Threading.Tasks;
using BizService.Models;

namespace BizService.Repositories
{
    public interface IUsersRepository
    {
        Task<User> GetUserAsync(string id);
        Task<List<User>> GetUsersAsync();
        Task SaveUserAsync(User user);
        Task<bool> IsExistAsync(string id);
    }
}