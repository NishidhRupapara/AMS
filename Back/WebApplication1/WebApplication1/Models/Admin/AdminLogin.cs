using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Models.Admin
{
    public class AdminLogin
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }  // MongoDB's _id (auto-generated)
        
        // ✅ FIXED: Changed from 'string' to 'int'
        public int Aid { get; set; }  
        
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;  // Store hashed password in production!
        
        // Optional: Add timestamps or other fields if needed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}