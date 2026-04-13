using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WebApplication1.Models
{
    // This is the one we will use in the Faculty Controller
    [BsonIgnoreExtraElements]
    public class FacultyLoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class StudentLoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class Counter
    {
        [BsonId]
        public string Id { get; set; } = string.Empty;
        public int Seq { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class LeaveRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string FacultyId { get; set; } = string.Empty;
        public string LeaveType { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime AppliedOn { get; set; }
        public string AdminRemark { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class StudyMaterial
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string FacultyId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public DateTime PostedOn { get; set; }
    }

    [BsonIgnoreExtraElements]
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
        public DateTime Deadline { get; set; }
        public DateTime PostedOn { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ExamMark
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string FacultyId { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public int MarksObtained { get; set; }
        public int TotalMarks { get; set; }
        public string ExamType { get; set; } = string.Empty;
        public DateTime DateEntered { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class F_Suggestion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int SuggestionId { get; set; }
        public string FacultyId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
        public string Status { get; set; } = "Pending";
        public string Reply { get; set; } = string.Empty;
    }
}