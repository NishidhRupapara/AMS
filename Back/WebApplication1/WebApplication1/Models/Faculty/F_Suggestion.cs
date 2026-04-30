using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Faculty
{
    public class F_Suggestion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SuggestionId { get; set; }

        public string FacultyId { get; set; } = "0"; // Store ID as string for flexibility

        public string Target { get; set; } = "Faculty"; // "Faculty" or "Admin"
        
        public string TargetName { get; set; } = "Unknown"; // Teacher Name or "Admin"

        public string StudentName { get; set; } = "Student"; // Sender Name

        public string StudentId { get; set; } = "0"; // Sender ID for filtering history

        [Required]
        [MaxLength(200)]
        public string? Title { get; set; }

        [Required]
        public string? Message { get; set; }

        public DateTime PostedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending";

        public string Reply { get; set; } = "Not Yet !";
    }
}
