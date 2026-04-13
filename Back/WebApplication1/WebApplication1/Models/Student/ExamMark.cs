using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace YourNamespace.Models
{
    public class ExamMark
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public int FacultyId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        
        public string Department { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty; // e.g., Mid-Term, Final
        
        public int MarksObtained { get; set; }
        public int TotalMarks { get; set; }

        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}