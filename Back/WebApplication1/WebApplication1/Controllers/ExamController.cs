using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamController : ControllerBase
    {
        private readonly IMongoCollection<Exam> _exams;
        private readonly IMongoCollection<ExamAttempt> _attempts;
        private readonly IMongoCollection<ExamMark> _results;

        public ExamController(IMongoClient mongoClient, IConfiguration config)
        {
            var databaseName = config["MongoDB:Database"] ?? "AMS";
            var database = mongoClient.GetDatabase(databaseName);
            _exams = database.GetCollection<Exam>("Exams");
            _attempts = database.GetCollection<ExamAttempt>("ExamAttempts");
            _results = database.GetCollection<ExamMark>("ExamMarks");
        }

        // --- Teacher Endpoints ---

        [HttpPost("create")]
        public async Task<IActionResult> CreateExam([FromBody] Exam exam)
        {
            try
            {
                if (exam == null) return BadRequest();
                exam.CreatedAt = DateTime.UtcNow;
                await _exams.InsertOneAsync(exam);
                return Ok(new { message = "Exam created successfully!", examId = exam.Id });
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("faculty/{facultyId}")]
        public async Task<IActionResult> GetFacultyExams(string facultyId)
        {
            var list = await _exams.Find(e => e.FacultyId == facultyId).SortByDescending(e => e.CreatedAt).ToListAsync();
            return Ok(list);
        }

        [HttpPut("publish/{examId}")]
        public async Task<IActionResult> PublishResults(string examId)
        {
            var update = Builders<Exam>.Update.Set(e => e.IsPublished, true);
            await _exams.UpdateOneAsync(e => e.Id == examId, update);
            return Ok(new { message = "Results published successfully!" });
        }

        // --- Student Endpoints ---

        [HttpGet("available/{dept}")]
        public async Task<IActionResult> GetAvailableExams(string dept)
        {
            // Show exams for their department that are scheduled for today or upcoming
            var list = await _exams.Find(e => e.Department == dept).ToListAsync();
            return Ok(list);
        }

        [HttpGet("details/{examId}")]
        public async Task<IActionResult> GetExamDetails(string examId)
        {
            var exam = await _exams.Find(e => e.Id == examId).FirstOrDefaultAsync();
            if (exam == null) return NotFound();
            return Ok(exam);
        }

        [HttpPost("attempt")]
        public async Task<IActionResult> SubmitAttempt([FromBody] ExamAttempt attempt)
        {
            try
            {
                if (attempt == null) return BadRequest();
                
                // Fetch exam to calculate score
                var exam = await _exams.Find(e => e.Id == attempt.ExamId).FirstOrDefaultAsync();
                if (exam == null) return NotFound("Exam not found");

                int score = 0;
                int totalPossible = 0;

                for (int i = 0; i < exam.Questions.Count; i++)
                {
                    totalPossible += exam.Questions[i].Marks;
                    if (attempt.SelectedOptions.Count > i && attempt.SelectedOptions[i] == exam.Questions[i].CorrectOptionIndex)
                    {
                        score += exam.Questions[i].Marks;
                    }
                }

                attempt.Score = score;
                attempt.TotalMarks = totalPossible;
                // Check for expiry: current time must be before (ExamDate + Duration)
                var expiryTime = exam.ExamDate.AddMinutes(exam.DurationMinutes);
                if (DateTime.UtcNow > expiryTime)
                {
                    return BadRequest(new { message = "Exam time has expired. Submissions are no longer accepted." });
                }

                await _attempts.InsertOneAsync(attempt);

                // Automatically push to ExamMarks for visibility in Results component
                var mark = new ExamMark
                {
                    ExamId = exam.Id ?? "",
                    StudentId = attempt.StudentId,
                    FacultyId = exam.FacultyId,
                    Subject = exam.Subject,
                    MarksObtained = score,
                    TotalMarks = totalPossible,
                    ExamType = exam.Title,
                    DateEntered = DateTime.UtcNow
                };
                await _results.InsertOneAsync(mark);

                return Ok(new { message = "Exam submitted successfully!", score, totalPossible });
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // --- Admin Endpoints ---

        [HttpGet("all")]
        public async Task<IActionResult> GetAllExams()
        {
            return Ok(await _exams.Find(_ => true).ToListAsync());
        }

        [HttpGet("attempts/{examId}")]
        public async Task<IActionResult> GetExamAttempts(string examId)
        {
            var list = await _attempts.Find(a => a.ExamId == examId).ToListAsync();
            return Ok(list);
        }

        [HttpGet("results/student/{studentId}")]
        public async Task<IActionResult> GetStudentResults(string studentId)
        {
            try
            {
                // Fetch all marks for this student
                var marks = await _results.Find(r => r.StudentId == studentId).ToListAsync();
                
                // Fetch all published exams once
                var publishedExams = await _exams.Find(e => e.IsPublished == true).ToListAsync();
                var publishedExamIds = publishedExams.Select(e => e.Id).ToHashSet();

                // Filter marks: Only show those where the associated exam is published
                var filteredResults = marks.Where(m => 
                    (!string.IsNullOrEmpty(m.ExamId) && publishedExamIds.Contains(m.ExamId)) ||
                    (string.IsNullOrEmpty(m.ExamId) && publishedExams.Any(e => e.Title == m.ExamType && e.Subject == m.Subject))
                ).ToList();

                return Ok(filteredResults);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExam(string id)
        {
            try
            {
                var result = await _exams.DeleteOneAsync(e => e.Id == id);
                if (result.DeletedCount == 0) return NotFound();
                return Ok(new { message = "Exam deleted successfully!" });
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}
