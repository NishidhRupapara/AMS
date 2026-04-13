using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class ActivityLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [BsonElement("activity")]
        [JsonPropertyName("activity")]
        public string Activity { get; set; } = string.Empty;

        [BsonElement("date")]
        [JsonPropertyName("date")]
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}