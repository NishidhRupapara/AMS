using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Models
{
    public class TestModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } 


        [BsonElement("Name")]
        public string? Name { get; set; }

        [BsonElement("Age")]
        public int Age { get; set; }

        [BsonElement("City")]
        public string? City { get; set; }
    }
}
