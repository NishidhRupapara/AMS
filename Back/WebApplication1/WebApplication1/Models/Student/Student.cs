using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WebApplication1.Models.Student
{
    [BsonIgnoreExtraElements] // ✅ 1. ADD THIS: Ignores extra fields from Angular
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Sid")]
        public int Sid { get; set; }

        [BsonElement("Faculty_Id")] 
        public string Faculty_Id { get; set; } = string.Empty;

        [BsonElement("Fname")]
        public string Fname { get; set; } = string.Empty;

        [BsonElement("Mname")]
        public string Mname { get; set; } = string.Empty;

        [BsonElement("Lname")]
        public string Lname { get; set; } = string.Empty;

        [BsonElement("Gender")]
        public string Gender { get; set; } = string.Empty;

        [BsonElement("DOB")]
        public DateTime? DOB { get; set; } // ✅ 2. CHANGE TO DateTime?: Prevents 400 on empty dates

        [BsonElement("DOA")]
        public DateTime? DOA { get; set; } // ✅ 3. CHANGE TO DateTime?: Prevents 400 on empty dates

        [BsonElement("Email_Id")]
        public string Email_Id { get; set; } = string.Empty;

        [BsonElement("Mo_Number")]
        public string Mo_Number { get; set; } = string.Empty;

        [BsonElement("Address")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("Department")]
        public string Department { get; set; } = string.Empty; 

        [BsonElement("ParentName")]
        public string ParentName { get; set; } = string.Empty;

        [BsonElement("ParentMobile")]
        public string ParentMobile { get; set; } = string.Empty;

        [BsonElement("ParentEmail")]
        public string ParentEmail { get; set; } = string.Empty;

        [BsonElement("Password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("ImageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}