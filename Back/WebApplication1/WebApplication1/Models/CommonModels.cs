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
        public string? FileName { get; set; }
        public string? FileData { get; set; }
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
        public string ReferenceLink { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public string? FileName { get; set; }
        public string? FileData { get; set; }
        public DateTime PostedOn { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ExamMark
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string ExamId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string FacultyId { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public int MarksObtained { get; set; }
        public int TotalMarks { get; set; }
        public string ExamType { get; set; } = string.Empty;
        public DateTime DateEntered { get; set; }
    }


    [BsonIgnoreExtraElements]
    public class Exam
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string FacultyId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public DateTime ExamDate { get; set; }
        public List<Question> Questions { get; set; } = new();
        public bool IsPublished { get; set; } = false;
        public DateTime CreatedAt { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Question
    {
        public string QuestionText { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public int CorrectOptionIndex { get; set; }
        public int Marks { get; set; } = 1;
    }

    [BsonIgnoreExtraElements]
    public class ExamAttempt
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string ExamId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public List<int> SelectedOptions { get; set; } = new();
        public int Score { get; set; }
        public int TotalMarks { get; set; }
        public DateTime AttemptedAt { get; set; }
        public bool IsEvaluated { get; set; } = true;
    }
}