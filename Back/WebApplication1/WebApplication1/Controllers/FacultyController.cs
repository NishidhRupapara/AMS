using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Models.Faculty;
using WebApplication1.Models.Student;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacultyController : ControllerBase
    {
        private readonly IMongoCollection<Faculty> _faculties;
        private readonly IMongoCollection<BsonDocument> _counters;
        private readonly IMongoCollection<F_Suggestion> _suggestions;
        private readonly IMongoCollection<Student> _students;
        private readonly IMongoCollection<StudyMaterial> _studyMaterials;
        private readonly IMongoCollection<Assignment> _assignments;
        private readonly IMongoCollection<LeaveRequest> _leaves;
        private readonly IMongoCollection<ExamMark> _examMarks;

        public FacultyController(IConfiguration config, IMongoClient mongoClient)
        {
            var databaseName = config["MongoDB:Database"] ?? "AMS";
            var database = mongoClient.GetDatabase(databaseName);

            _faculties = database.GetCollection<Faculty>("faculties");
            _students = database.GetCollection<Student>("StudentTbl");
            _suggestions = database.GetCollection<F_Suggestion>("F_Suggestion");
            _studyMaterials = database.GetCollection<StudyMaterial>("StudyMaterials");
            _assignments = database.GetCollection<Assignment>("Assignments");
            _leaves = database.GetCollection<LeaveRequest>("LeaveRequests");
            _examMarks = database.GetCollection<ExamMark>("ExamMarks");
            _counters = database.GetCollection<BsonDocument>("Counters");
        }

        // --- Helper: Atomic Auto-Increment ---
        private long GetNextId(string sequenceName)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", sequenceName);
            var update = Builders<BsonDocument>.Update.Inc("Seq", 1L);
            var options = new FindOneAndUpdateOptions<BsonDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };
            var result = _counters.FindOneAndUpdate(filter, update, options);
            return result["Seq"].ToInt64();
        }

        // --- Auth Management ---

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] WebApplication1.Models.FacultyLoginModel login)
        {
            if (login == null || string.IsNullOrEmpty(login.Email))
                return BadRequest(new { message = "Invalid data received." });

            var faculty = await _faculties.Find(f => 
                (f.Email == login.Email || f.Username == login.Email) && 
                f.Password == login.Password).FirstOrDefaultAsync();
            
            if (faculty == null) 
                return Unauthorized(new { message = "Invalid Credentials" });

            return Ok(new { Fid = faculty.Fid, Id = faculty.Id, Username = faculty.Username });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterFaculty([FromBody] Faculty faculty)
        {
            if (faculty == null) return BadRequest(new { message = "Invalid data" });
            faculty.Fid = GetNextId("facultyid");
            await _faculties.InsertOneAsync(faculty);
            return Ok(faculty);
        }

        // --- Classroom & Materials Management ---

        [HttpPost("study-material")]
        public async Task<IActionResult> PostStudyMaterial([FromBody] StudyMaterial material)
        {
            if (material == null) return BadRequest();
            material.PostedOn = DateTime.Now;
            await _studyMaterials.InsertOneAsync(material);
            return Ok(new { message = "✅ Study Material shared successfully!" });
        }

        [HttpGet("my-materials/{facultyId}")]
        public async Task<IActionResult> GetMyMaterials(string facultyId)
        {
            try
            {
                var filterBuilder = Builders<StudyMaterial>.Filter;
                var filter = filterBuilder.Eq(m => m.FacultyId, facultyId);

                // If facultyId is a number or there are legacy records, check both forms
                if (long.TryParse(facultyId, out long fid))
                {
                    filter = filterBuilder.Or(filter, filterBuilder.Eq(m => m.FacultyId, fid.ToString()));
                }

                var materials = await _studyMaterials
                    .Find(filter)
                    .SortByDescending(m => m.PostedOn)
                    .ToListAsync();

                return Ok(materials ?? new List<StudyMaterial>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving history", error = ex.Message });
            }
        }

        [HttpPost("post-assignment")]
        public async Task<IActionResult> PostAssignment([FromBody] Assignment assignment)
        {
            if (assignment == null) return BadRequest();
            assignment.PostedOn = DateTime.Now;
            await _assignments.InsertOneAsync(assignment);
            return Ok(new { message = "✅ Assignment posted successfully!" });
        }

        [HttpGet("my-assignments/{facultyId}")]
        public async Task<IActionResult> GetMyAssignments(string facultyId)
        {
            try
            {
                var filterBuilder = Builders<Assignment>.Filter;
                var filter = filterBuilder.Eq(a => a.FacultyId, facultyId);

                if (long.TryParse(facultyId, out long fid))
                {
                    filter = filterBuilder.Or(filter, filterBuilder.Eq(a => a.FacultyId, fid.ToString()));
                }

                var assignments = await _assignments
                    .Find(filter)
                    .SortByDescending(a => a.PostedOn)
                    .ToListAsync();

                return Ok(assignments ?? new List<Assignment>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving history", error = ex.Message });
            }
        }

        [HttpGet("departments")]
        public async Task<IActionResult> GetUniqueDepartments()
        {
            try
            {
                var departments = await _students.Distinct<string>("Department", Builders<Student>.Filter.Empty).ToListAsync();
                return Ok(departments ?? new List<string>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching departments", error = ex.Message });
            }
        }

        // --- Suggestions & Feedback ---

        [HttpPost("suggestion")]
        public async Task<IActionResult> PostSuggestion([FromBody] F_Suggestion suggestion)
        {
            if (suggestion == null) return BadRequest();
            suggestion.SuggestionId = (int)GetNextId("suggestionid");
            suggestion.PostedAt = DateTime.Now;
            suggestion.Status = "Pending";
            suggestion.Reply = "Not Yet !";
            await _suggestions.InsertOneAsync(suggestion);
            return Ok(new { message = "Submitted successfully", suggestion });
        }

        [HttpGet("AllSuggestionFaculty")]
        public async Task<IActionResult> GetAllSuggestion()
        {
            try 
            {
                var list = await _suggestions.Find(_ => true).ToListAsync();
                var facultys = await _faculties.Find(_ => true).ToListAsync();

                var result = list.Select(s => {
                    var factName = "Admin";
                    if (s.Target == "Faculty" && s.FacultyId != "0")
                    {
                        factName = facultys.Where(f => f.Fid.ToString() == s.FacultyId || f.Id == s.FacultyId)
                                           .Select(f => $"{f.Fname} {f.Lname}")
                                           .FirstOrDefault() ?? "Unknown";
                    }
                    else if (s.TargetName != "Admin" && s.TargetName != "Unknown" && !string.IsNullOrEmpty(s.TargetName))
                    {
                        factName = s.TargetName;
                    }

                    return new {
                        id = s.Id,
                        suggestionId = s.SuggestionId,
                        facultyId = s.FacultyId,
                        target = s.Target,
                        studentName = s.StudentName,
                        studentId = s.StudentId,
                        title = s.Title,
                        message = s.Message,
                        postedAt = s.PostedAt,
                        status = s.Status,
                        reply = s.Reply,
                        targetName = factName
                    };
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("ViewSuggestion")]
        public async Task<IActionResult> ViewSuggestion([FromQuery] string Facultyid)
        {
            try
            {
                // Filter: Suggestions for this Faculty OR Suggestions for Admin (Universal visibility)
                // Using typed model class is 100% reliable
                var list = await _suggestions.Find(s => 
                    s.FacultyId == Facultyid || 
                    s.FacultyId == "0" || 
                    s.Target == "Admin"
                ).ToListAsync();

                var facultys = await _faculties.Find(_ => true).ToListAsync();

                var result = list.Select(s => {
                    var factName = "Admin";
                    if (s.Target == "Faculty" && s.FacultyId != "0")
                    {
                        factName = facultys.Where(f => f.Fid.ToString() == s.FacultyId || f.Id == s.FacultyId)
                                           .Select(f => $"{f.Fname} {f.Lname}")
                                           .FirstOrDefault() ?? "Unknown";
                    }
                    else if (s.TargetName != "Admin" && s.TargetName != "Unknown" && !string.IsNullOrEmpty(s.TargetName))
                    {
                        factName = s.TargetName;
                    }

                    return new {
                        id = s.Id,
                        suggestionId = s.SuggestionId,
                        facultyId = s.FacultyId,
                        target = s.Target,
                        targetName = factName,
                        studentName = s.StudentName,
                        studentId = s.StudentId,
                        title = s.Title,
                        message = s.Message,
                        postedAt = s.PostedAt,
                        status = s.Status,
                        reply = s.Reply
                    };
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // --- Leaves & Marks ---

        [HttpPost("apply-leave")]
        public async Task<IActionResult> ApplyLeave([FromBody] LeaveRequest leave)
        {
            if (leave == null) return BadRequest();
            leave.AppliedOn = DateTime.Now;
            leave.Status = "Pending";
            await _leaves.InsertOneAsync(leave);
            return Ok(new { message = "Leave application submitted successfully." });
        }

        [HttpGet("my-leaves/{facultyId}")]
        public async Task<IActionResult> GetMyLeaves(string facultyId)
        {
            try
            {
                var filterBuilder = Builders<LeaveRequest>.Filter;
                var filter = filterBuilder.Eq(a => a.FacultyId, facultyId);

                if (long.TryParse(facultyId, out long fid))
                {
                    filter = filterBuilder.Or(filter, filterBuilder.Eq(a => a.FacultyId, fid.ToString()));
                }

                var leaves = await _leaves.Find(filter).SortByDescending(l => l.AppliedOn).ToListAsync();
                return Ok(leaves ?? new List<LeaveRequest>());
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPost("submit-marks")]
        public async Task<IActionResult> SubmitMarks([FromBody] List<ExamMark> marks)
        {
            if (marks == null || !marks.Any()) return BadRequest(new { message = "No data" });
            foreach (var m in marks) m.DateEntered = DateTime.Now;
            await _examMarks.InsertManyAsync(marks);
            return Ok(new { message = "Marks submitted successfully!" });
        }

        // --- Profile Management ---

        [HttpGet("profile/{fid}")]
        public async Task<IActionResult> GetFacultyProfile(string fid)
        {
            try
            {
                var filterBuilder = Builders<Faculty>.Filter;
                var filter = filterBuilder.Eq(f => f.Id, fid);

                if (long.TryParse(fid, out long numericFid))
                {
                    filter = filterBuilder.Or(filter, filterBuilder.Eq(f => f.Fid, numericFid));
                }
                var faculty = await _faculties.Find(filter).FirstOrDefaultAsync();
                if (faculty == null) return NotFound(new { message = "Faculty profile not found." });
                return Ok(faculty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching profile", error = ex.Message });
            }
        }

        [HttpPut("profile/{fid}")]
        public async Task<IActionResult> UpdateFacultyProfile(string fid, [FromBody] Faculty updatedFaculty)
        {
            try
            {
                var filterBuilder = Builders<Faculty>.Filter;
                var filter = filterBuilder.Eq(f => f.Id, fid);

                if (long.TryParse(fid, out long numericFid))
                {
                    filter = filterBuilder.Or(filter, filterBuilder.Eq(f => f.Fid, numericFid));
                }
                
                var update = Builders<Faculty>.Update
                    .Set(f => f.Fname, updatedFaculty.Fname)
                    .Set(f => f.Mname, updatedFaculty.Mname)
                    .Set(f => f.Lname, updatedFaculty.Lname)
                    .Set(f => f.Username, updatedFaculty.Username)
                    .Set(f => f.Email, updatedFaculty.Email)
                    .Set(f => f.Mobile, updatedFaculty.Mobile)
                    .Set(f => f.Gender, updatedFaculty.Gender)
                    .Set(f => f.Department, updatedFaculty.Department);

                var result = await _faculties.UpdateOneAsync(filter, update);
                if (result.MatchedCount == 0) return NotFound(new { message = "Faculty not found." });

                return Ok(new { message = "Profile updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Update failed", error = ex.Message });
            }
        }

        // --- General Faculty List & Delete ---

        [HttpGet("all")]
        public async Task<IActionResult> GetAllFaculty()
        {
            var allFaculties = await _faculties.Find(_ => true).ToListAsync();
            return Ok(allFaculties);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFaculty(string id)
        {
            try
            {
                var filterBuilder = Builders<Faculty>.Filter;
                FilterDefinition<Faculty> filter;

                if (ObjectId.TryParse(id, out _))
                {
                    filter = filterBuilder.Eq(f => f.Id, id);
                }
                else if (long.TryParse(id, out long numericFid))
                {
                    filter = filterBuilder.Eq(f => f.Fid, numericFid);
                }
                else
                {
                    return BadRequest(new { message = "Invalid API ID format." });
                }

                var result = await _faculties.DeleteOneAsync(filter);
                if (result.DeletedCount == 0) return NotFound(new { message = "Faculty not found." });

                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("students-by-dept/{dept}")]
        public async Task<IActionResult> GetStudentsByDept(string dept)
        {
            try
            {
                var students = await _students.Find(s => s.Department == dept).ToListAsync();
                return Ok(students);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}