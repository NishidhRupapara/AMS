using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Models.Admin
{
    public class NoticeAdmin
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("NoticeId")]
        public int NoticeId { get; set; }  // Auto-increment ID

        [BsonElement("NoticeTitle")]
        public string NoticeTitle { get; set; } = string.Empty;

        [BsonElement("NoticeMessage")]
        public string NoticeMessage { get; set; } = string.Empty;

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
