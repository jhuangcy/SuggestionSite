using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuggestionSiteLib.Models
{
    public class UserLite
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public UserLite()
        {
            
        }

        public UserLite(User user)
        {
            Id = user.Id;
            DisplayName = user.DisplayName;
        }
    }
}
