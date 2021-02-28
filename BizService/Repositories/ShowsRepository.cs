using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using BizService.Models;

namespace BizService.Repositories
{
    public class ShowsRepository : IShowsRepository
    {
        private IDynamoDBContext _context;

        public ShowsRepository(IDynamoDBContext context)
        {
            _context = context;
        }

        public Task AddShowAsync(Show show)
        {
            show.Id = Guid.NewGuid().ToString();
            return _context.SaveAsync<Show>(show);
        }

        public Task<Show> GetShowAsync(string id)
        {
            return _context.LoadAsync<Show>(id);
        }

        public Task<List<Show>> GetShowsAsync()
        {
            return _context.ScanAsync<Show>(new List<ScanCondition>()).GetRemainingAsync();
        }
    }
}