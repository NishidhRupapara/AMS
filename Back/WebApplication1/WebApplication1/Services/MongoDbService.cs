using MongoDB.Driver;
using WebApplication1.Models;
using WebApplication1.Models.Faculty; // Added this line to fix the CS0246 error

namespace WebApplication1.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<Faculty> _facultyCollection;

        public MongoDbService(IConfiguration config)
        {
            var connectionString = config["MongoDB:ConnectionString"];
            var databaseName = config["MongoDB:Database"];
            var facultyCollectionName = config["MongoDB:FacultyCollectionName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            _facultyCollection = database.GetCollection<Faculty>(facultyCollectionName);
        }

        public async Task<List<Faculty>> GetAllFacultyAsync()
        {
            return await _facultyCollection.Find(_ => true).ToListAsync();
        }

        public async Task AddFacultyAsync(Faculty faculty)
        {
            await _facultyCollection.InsertOneAsync(faculty);
        }
    }
}