using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WebApplication1.Models.Student
{
    [BsonIgnoreExtraElements] // ✅ Prevents crashes if extra fields exist in MongoDB
    public class StudentAttendance
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] 
        public string? Id { get; set; } // Matches MongoDB _id

        [BsonElement("StudentId")]
        public object? StudentIdObj { get; set; }

        [BsonIgnore]
        public string StudentId 
        { 
            get => StudentIdObj?.ToString() ?? string.Empty; 
            set => StudentIdObj = value; 
        }

        [BsonElement("FacultyId")]
        public object? FacultyIdObj { get; set; }

        [BsonIgnore]
        public string FacultyId 
        { 
            get => FacultyIdObj?.ToString() ?? string.Empty; 
            set => FacultyIdObj = value; 
        }

        [BsonElement("Status")]
        public string Status { get; set; } = string.Empty; // e.g., "Present", "Absent"

        [BsonElement("Remark")]
        public string? Remark { get; set; } // Optional notes

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("Date")]
        public DateTime Date { get; set; } = DateTime.UtcNow; // Standardized UTC time
    }
}