using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuggestionSiteLib.Services
{
    public interface IDbConfig
    {
        MongoClient Client { get; }
        string DbName { get; }
        string CategoryCollection { get; }
        string StatusCollection { get; }
        string UserCollection { get; }
        string SuggestionCollection { get; }
        IMongoCollection<Category> Categories { get; }
        IMongoCollection<Status> Statuses { get; }
        IMongoCollection<User> Users { get; }
        IMongoCollection<Suggestion> Suggestions { get; }
    }

    public class DbConfig : IDbConfig
    {
        private readonly IConfiguration _config;
        private readonly IMongoDatabase _db;
        private string _conId = "MongoDb";   // from appsettings

        public MongoClient Client { get; private set; }
        public string DbName { get; private set; }

        public string CategoryCollection { get; private set; } = "Categories";
        public string StatusCollection { get; private set; } = "Statuses";
        public string UserCollection { get; private set; } = "Users";
        public string SuggestionCollection { get; private set; } = "Suggestions";
        public IMongoCollection<Category> Categories { get; private set; }
        public IMongoCollection<Status> Statuses { get; private set; }
        public IMongoCollection<User> Users { get; private set; }
        public IMongoCollection<Suggestion> Suggestions { get; private set; }

        public DbConfig(IConfiguration config)
        {
            _config = config;

            Client = new MongoClient(_config.GetConnectionString(_conId));
            DbName = _config["DbName"];
            _db = Client.GetDatabase(DbName);

            Categories = _db.GetCollection<Category>(CategoryCollection);
            Statuses = _db.GetCollection<Status>(StatusCollection);
            Users = _db.GetCollection<User>(UserCollection);
            Suggestions = _db.GetCollection<Suggestion>(SuggestionCollection);
        }
    }
}
