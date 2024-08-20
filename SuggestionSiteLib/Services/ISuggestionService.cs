using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuggestionSiteLib.Services
{
    public interface ISuggestionService
    {
        Task<List<Suggestion>> FindAllNonArchivedAsync();
        Task<List<Suggestion>> FindAllFromUserAsync(string userId);
        List<Suggestion> FindAllApproved();
        List<Suggestion> FindAllNonApproved();
        Task<Suggestion> FindOneAsync(string id);
        Task InsertOneAsync(Suggestion suggestion);
        Task ReplaceOneAsync(Suggestion suggestion);
        Task VoteAsync(string suggestionId, string userId);
    }

    public class SuggestionService : ISuggestionService
    {
        private const string CACHE_NAME = "Suggestions";
        private readonly IMemoryCache _cache;
        private readonly IDbConfig _db;
        private readonly IUserService _userService;
        private readonly IMongoCollection<Suggestion> _suggestions;

        public SuggestionService(IMemoryCache cache, IDbConfig db, IUserService userService)
        {
            _cache = cache;
            _db = db;
            _userService = userService;
            _suggestions = _db.Suggestions;
        }

        public async Task<List<Suggestion>> FindAllNonArchivedAsync()
        {
            var res = _cache.Get<List<Suggestion>>(CACHE_NAME);
            if (res == null)
            {
                res = await _suggestions.FindAsync(s => !s.Archived).Result.ToListAsync();
                _cache.Set(CACHE_NAME, res, TimeSpan.FromMinutes(1));
            }
            return res;
        }

        public async Task<List<Suggestion>> FindAllFromUserAsync(string userId)
        {
            var res = _cache.Get<List<Suggestion>>(userId);
            if (res == null)
            { 
                res = await _suggestions.FindAsync(s => s.Author.Id == userId).Result.ToListAsync();
                _cache.Set(userId, res, TimeSpan.FromMinutes(1));
            }
            return res;
        }

        public List<Suggestion> FindAllApproved() => FindAllNonArchivedAsync().Result.Where(s => s.Approved).ToList();
        public List<Suggestion> FindAllNonApproved() => FindAllNonArchivedAsync().Result.Where(s => !s.Approved && !s.Rejected).ToList();

        // https://stackoverflow.com/questions/61783999/how-to-validate-a-mongo-objectid-in-net
        public async Task<Suggestion> FindOneAsync(string id) => ObjectId.TryParse(id, out _) == true ? await _suggestions.FindAsync(s => s.Id == id).Result.FirstOrDefaultAsync() : null;

        public async Task InsertOneAsync(Suggestion suggestion)
        {
            using var session = await _db.Client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                // get db snapshot
                var db = _db.Client.GetDatabase(_db.DbName);
                var suggestions = db.GetCollection<Suggestion>(_db.SuggestionCollection);
                var users = db.GetCollection<User>(_db.UserCollection);

                // handle suggestion
                await suggestions.InsertOneAsync(session, suggestion);

                // handle user
                var user = await _userService.FindOneAsync(suggestion.Author.Id);
                user.Authored.Add(new SuggestionLite(suggestion));
                await users.ReplaceOneAsync(session, u => u.Id == user.Id, user);

                // commit
                await session.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                throw;
            }
        }

        public async Task ReplaceOneAsync(Suggestion suggestion)
        {
            await _suggestions.ReplaceOneAsync(s => s.Id == suggestion.Id, suggestion);
            _cache.Remove(CACHE_NAME);
        }

        public async Task VoteAsync(string suggestionId, string userId)
        {
            using var session = await _db.Client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                // get db snapshot
                var db = _db.Client.GetDatabase(_db.DbName);
                var suggestions = db.GetCollection<Suggestion>(_db.SuggestionCollection);
                var users = db.GetCollection<User>(_db.UserCollection);

                // handle suggestion
                var suggestion = await suggestions.FindAsync(s => s.Id == suggestionId).Result.FirstAsync();
                var voted = suggestion.Votes.Add(userId);
                if (!voted) suggestion.Votes.Remove(userId);
                await suggestions.ReplaceOneAsync(session, s => s.Id == suggestionId, suggestion);

                // handle user
                var user = await _userService.FindOneAsync(userId);
                if (voted) user.Voted.Add(new SuggestionLite(suggestion));
                else
                {
                    var s = user.Voted.First(s => s.Id == suggestionId);
                    user.Voted.Remove(s);
                }
                await users.ReplaceOneAsync(session, u => u.Id == userId, user);

                // commit
                await session.CommitTransactionAsync();
                _cache.Remove(CACHE_NAME);
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                throw;
            }
        }
    }
}
