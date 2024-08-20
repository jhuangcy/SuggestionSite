using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuggestionSiteLib.Models
{
    public class SuggestionLite
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public SuggestionLite()
        {
            
        }

        public SuggestionLite(Suggestion suggestion)
        {
            Id = suggestion.Id;
            Name = suggestion.Name;
        }
    }
}
