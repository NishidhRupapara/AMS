using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourNamespace.Models
{
    public class F_Suggestion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto Increment
        public int SuggestionId { get; set; }

        [Required]
        public int FacultyId { get; set; }

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
