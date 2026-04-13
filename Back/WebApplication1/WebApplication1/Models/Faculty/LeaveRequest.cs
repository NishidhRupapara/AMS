using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace YourNamespace.Models
{
    public class LeaveRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public int FacultyId { get; set; }

        public string Reason { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }

        public string Status { get; set; } = "Pending"; 

        public string AdminRemark { get; set; } = "Awaiting review"; 

        public DateTime AppliedOn { get; set; } = DateTime.Now;
    }
}