using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuggestionSiteLib.Services
{
    public interface IStatusService
    {
        Task<List<Status>> FindAllAsync();
        Task InsertOneAsync(Status status);
    }

    public class StatusService : IStatusService
    {
        private const string CACHE_NAME = "Statuses";
        private readonly IMemoryCache _cache;
        private readonly IMongoCollection<Status> _statuses;

        public StatusService(IMemoryCache cache, IDbConfig db)
        {
            _cache = cache;
            _statuses = db.Statuses;
        }

        public async Task<List<Status>> FindAllAsync()
        {
            var res = _cache.Get<List<Status>>(CACHE_NAME) ?? await _statuses.FindAsync(_ => true).Result.ToListAsync();
            _cache.Set(CACHE_NAME, res, TimeSpan.FromDays(1));
            return res;
        }

        public async Task InsertOneAsync(Status status) => await _statuses.InsertOneAsync(status);
    }
}
