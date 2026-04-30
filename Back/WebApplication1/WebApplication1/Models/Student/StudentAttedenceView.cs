using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WebApplication1.Models.Student
{
    public class StudentAttedenceView
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string FacultyId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Remark { get; set; }
        public string? Fullname { get; set; } // ✅ Populated by the join logic
        public string? FacultyName { get; set; } // ✅ Populated by the join logic
        public string? Department { get; set; } // ✅ Populated by the join logic
        public string? ImageUrl { get; set; } // ✅ Populated by the join logic
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}