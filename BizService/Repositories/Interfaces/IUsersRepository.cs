using System.Threading.Tasks;
using BizService.Models;

namespace BizService.Repositories
{
    public interface IUsersRepository
    {
        Task<User> GetUserAsync(string id);
        Task SaveUserAsync(User user);
        Task<bool> IsExistAsync(string id);
    }
}