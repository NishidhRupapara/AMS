using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace YourNamespace.Models
{
    public class StudyMaterial
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string FacultyId { get; set; } = string.Empty;
        
        public string Department { get; set; } = string.Empty;
        
        public string Subject { get; set; } = string.Empty;
        
        public string Title { get; set; } = string.Empty;
        
        public string MaterialLink { get; set; } = string.Empty; // Google Drive, YouTube, etc.

        public DateTime PostedOn { get; set; } = DateTime.Now;
    }
}