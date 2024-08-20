namespace SuggestionSiteLib.Models
{
    public class Suggestion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public bool Approved { get; set; } = false;
        public bool Archived { get; set; } = false;
        public bool Rejected { get; set; } = false;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public Category Category { get; set; }
        public Status Status { get; set; }
        public UserLite Author { get; set; }

        public HashSet<string> Votes { get; set; } = new(); // no dupes
    }
}
