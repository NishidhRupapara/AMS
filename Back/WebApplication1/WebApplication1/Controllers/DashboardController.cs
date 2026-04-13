using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebApplication1.Models; // Ensure this points to where ActivityLog.cs is

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IMongoDatabase _database;

        public DashboardController(IMongoClient mongoClient, IConfiguration config)
        {
            var databaseName = config["MongoDB:Database"] ?? "AMS";
            _database = mongoClient.GetDatabase(databaseName);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var studentCollection = _database.GetCollection<BsonDocument>("StudentTbl");
                var facultyCollection = _database.GetCollection<BsonDocument>("faculties");
                var departmentCollection = _database.GetCollection<BsonDocument>("Department"); // Check this exact name!
                
                // ✅ NEW: Connect to the ActivityLogs collection
                var activityCollection = _database.GetCollection<ActivityLog>("ActivityLogs");

                var totalStudents = await studentCollection.CountDocumentsAsync(new BsonDocument());
                var totalFaculty = await facultyCollection.CountDocumentsAsync(new BsonDocument());
                var totalDepartments = await departmentCollection.CountDocumentsAsync(new BsonDocument());

                // ✅ NEW: Fetch the 5 most recent activities, sorted by Date (Newest first)
                var recentActivities = await activityCollection.Find(_ => true)
                                        .SortByDescending(a => a.Date)
                                        .Limit(5)
                                        .ToListAsync();

                return Ok(new 
                {
                    totalStudents = totalStudents,
                    totalFaculty = totalFaculty,
                    totalDepartments = totalDepartments,
                    recentActivities = recentActivities // Send the list to React
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load dashboard stats", error = ex.Message });
            }
        }

        // ✅ Helper endpoint to test adding an activity manually
        [HttpPost("log-activity")]
        public async Task<IActionResult> LogActivity([FromBody] ActivityLog log)
        {
            var activityCollection = _database.GetCollection<ActivityLog>("ActivityLogs");
            log.Date = DateTime.UtcNow;
            await activityCollection.InsertOneAsync(log);
            return Ok(new { message = "Activity logged successfully!" });
        }
    }
}