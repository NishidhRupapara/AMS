using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Models.Faculty
{
    [BsonIgnoreExtraElements] // ✅ ESSENTIAL: Prevents crash on extra elements
    public class Faculty
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("fid")] 
        public long Fid { get; set; } 

        [BsonElement("FirstName")]
        public string Fname { get; set; } = string.Empty;

        [BsonElement("MiddleName")]
        public string Mname { get; set; } = string.Empty;

        [BsonElement("LastName")]
        public string Lname { get; set; } = string.Empty;

        [BsonElement("Department")]
        public string Department { get; set; } = string.Empty;

        [BsonElement("Username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("Password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("Email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("Mobile")]
        public string Mobile { get; set; } = string.Empty;

        [BsonElement("Gender")]
        public string Gender { get; set; } = string.Empty;

        [BsonElement("DateOfJoining")]
        public string Doj { get; set; } = string.Empty;

        [BsonElement("ImageUrl")]
        public string ImageUrl { get; set; } = string.Empty;
    }
}