using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuggestionSiteLib.Services
{
    public interface IUserService
    {
        Task<List<User>> FindAllAsync();
        Task<User> FindOneAsync(string id);
        Task<User> FindOneFromAuthAsync(string objId);   // for azure
        Task InsertOneAsync(User user);
        Task ReplaceOneAsync(User user);
    }

    public class UserService : IUserService
    {
        readonly IMongoCollection<User> _users;

        public UserService(IDbConfig db)
        {
            _users = db.Users;
        }

        public async Task<List<User>> FindAllAsync() => await _users.FindAsync(_ => true).Result.ToListAsync();
        public async Task<User> FindOneAsync(string id) => await _users.FindAsync(u => u.Id == id).Result.FirstOrDefaultAsync();
        public async Task<User> FindOneFromAuthAsync(string objId) => await _users.FindAsync(u => u.ObjectIdentifier == objId).Result.FirstOrDefaultAsync();
        public async Task InsertOneAsync(User user) => await _users.InsertOneAsync(user);
        public async Task ReplaceOneAsync(User user) => await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
        //{
        //    var filter = Builders<User>.Filter.Eq("Id", user.Id);
        //    await _users.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
        //}
    }
}
