using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Models
{
    [BsonIgnoreExtraElements]
    public class Departments // 🚀 FIXED: Changed from 'Department' to 'Departments' to match your Controller
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // 🚀 ADDED: Your controller requires 'Did' for the auto-increment logic
        [BsonElement("Did")]
        public int Did { get; set; }

        [BsonElement("DepartmentName")]
        public string DepartmentName { get; set; } = string.Empty;

        [BsonElement("ImageUrl")]
        public string ImageUrl { get; set; } = string.Empty;
    }
}