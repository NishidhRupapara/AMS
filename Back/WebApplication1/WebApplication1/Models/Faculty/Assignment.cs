using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace YourNamespace.Models
{
    public class Assignment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string FacultyId { get; set; } = string.Empty;
        
        public string Department { get; set; } = string.Empty;
        
        public string Subject { get; set; } = string.Empty;
        
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public string ReferenceLink { get; set; } = string.Empty; // Optional link

        public DateTime DueDate { get; set; }

        public DateTime PostedOn { get; set; } = DateTime.Now;
    }
}