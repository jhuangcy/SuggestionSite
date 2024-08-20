using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuggestionSiteLib.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string ObjectIdentifier { get; set; }    // from azure
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public List<SuggestionLite> Authored { get; set; } = new();
        public List<SuggestionLite> Voted { get; set; } = new();
    }
}
