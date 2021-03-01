using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using BizService.Models;

namespace BizService.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IDynamoDBContext _context;

        public UsersRepository(IDynamoDBContext context)
        {
            _context = context;
        }

        public Task<User> GetUserAsync(string id)
        {
            return _context.LoadAsync<User>(id);
        }

        public async Task<bool> IsExistAsync(string id)
        {
            var user = await _context.LoadAsync<User>(id);
            return user != null;
        }

        public Task SaveUserAsync(User user)
        {
            return _context.SaveAsync<User>(user);
        }
    }
}