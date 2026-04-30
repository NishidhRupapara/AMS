using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models.Faculty;
using WebApplication1.Models.Student;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IMongoCollection<Student> _student;
        private readonly IMongoCollection<StudentAttendance> _attendanceCollection;
        private readonly IMongoCollection<Counter> _counters;
        private readonly IMongoCollection<ExamMark> _examMarks;
        private readonly IMongoCollection<F_Suggestion> _suggestions;

        public StudentController(IMongoClient mongoClient, IConfiguration config)
        {
            var databaseName = config["MongoDB:Database"] ?? "AMS";
            var database = mongoClient.GetDatabase(databaseName);
            
            // ✅ Collection names match your MongoDB Compass exactly
            _student = database.GetCollection<Student>("StudentTbl");
            _attendanceCollection = database.GetCollection<StudentAttendance>("StudentAt");
            _counters = database.GetCollection<Counter>("counters");
            _examMarks = database.GetCollection<ExamMark>("ExamMarks");
            _suggestions = database.GetCollection<F_Suggestion>("F_Suggestion");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] StudentLoginDto login)
        {
            if (login == null || string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
                return BadRequest(new { message = "Invalid login data" });

            var student = await _student
                .Find(s => s.Email_Id == login.Email && s.Password == login.Password)
                .FirstOrDefaultAsync();

            if (student == null)
                return Unauthorized(new { message = "Invalid Email or Password" });

            return Ok(new 
            { 
                message = "Login successful", 
                Sid = student.Sid, 
                Name = $"{student.Fname} {student.Lname}".Trim(),
                Department = student.Department
            });
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitAttendance([FromBody] List<StudentAttendance> students)
        {
            if (students == null || !students.Any())
                return BadRequest(new { message = "No data provided" });

            try
            {
                var today = DateTime.UtcNow.Date;
                foreach (var att in students)
                {
                    att.Id = null; // Let MongoDB generate a new ObjectId
                    att.Date = DateTime.UtcNow;
                    await _attendanceCollection.InsertOneAsync(att);
                }
                return Ok(new { message = "Success" });
            }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpGet("history/{facultyId}")]
        public async Task<ActionResult<IEnumerable<StudentAttedenceView>>> GetAttendanceHistory(string facultyId)
        {
            try
            {
                var attendanceColRaw = _attendanceCollection.Database.GetCollection<BsonDocument>("StudentAt");
                var facultyCol = _attendanceCollection.Database.GetCollection<BsonDocument>("faculties");
                
                // 🚀 BROAD SEARCH: Build an OR filter for every possible ID permutation
                var possibleFacultyKeys = new[] { "FacultyId", "facultyId", "Faculty_Id", "faculty_id", "Fid", "fid" };
                var filterList = new List<FilterDefinition<BsonDocument>>();

                foreach (var key in possibleFacultyKeys)
                {
                    filterList.Add(Builders<BsonDocument>.Filter.Eq(key, facultyId));
                    if (ObjectId.TryParse(facultyId, out ObjectId oid))
                        filterList.Add(Builders<BsonDocument>.Filter.Eq(key, oid));
                    if (long.TryParse(facultyId, out long num))
                    {
                        filterList.Add(Builders<BsonDocument>.Filter.Eq(key, (int)num));
                        filterList.Add(Builders<BsonDocument>.Filter.Eq(key, num));
                        filterList.Add(Builders<BsonDocument>.Filter.Eq(key, num.ToString()));
                    }
                }

                var finalFilter = Builders<BsonDocument>.Filter.Or(filterList);
                var rawAttendanceList = await attendanceColRaw.Find(finalFilter).ToListAsync();

                // 🕵️ FALLBACK: If still empty, try to match by Faculty Name (Alan -> ID search)
                if (!rawAttendanceList.Any())
                {
                    var fDoc = await facultyCol.Find(Builders<BsonDocument>.Filter.Regex("FirstName", new BsonRegularExpression(facultyId, "i"))).FirstOrDefaultAsync();
                    if (fDoc == null) fDoc = await facultyCol.Find(Builders<BsonDocument>.Filter.Eq("_id", facultyId)).FirstOrDefaultAsync();

                    if (fDoc != null)
                    {
                        var altId = fDoc.Contains("fid") ? fDoc["fid"] : fDoc["_id"];
                        var altFilterList = new List<FilterDefinition<BsonDocument>>();
                        foreach (var key in possibleFacultyKeys)
                        {
                            altFilterList.Add(Builders<BsonDocument>.Filter.Eq(key, altId));
                            altFilterList.Add(Builders<BsonDocument>.Filter.Eq(key, altId.ToString()));
                        }
                        rawAttendanceList = await attendanceColRaw.Find(Builders<BsonDocument>.Filter.Or(altFilterList)).ToListAsync();
                    }
                }
                
                var studentList = await _student.Find(_ => true).ToListAsync();

                var result = rawAttendanceList.Select(doc =>
                {
                    var dict = doc.ToDictionary();
                    
                    // Robustly extract fields regardless of case or naming style
                    string sId = (dict.ContainsKey("StudentId") ? dict["StudentId"] : 
                                 dict.ContainsKey("studentId") ? dict["studentId"] : 
                                 dict.ContainsKey("Sid") ? dict["Sid"] : 
                                 dict.ContainsKey("sid") ? dict["sid"] : "").ToString() ?? "";
                                 
                    string status = (dict.ContainsKey("Status") ? dict["Status"] : 
                                    dict.ContainsKey("status") ? dict["status"] : "Present").ToString() ?? "Present";
                                    
                    string remark = (dict.ContainsKey("Remark") ? dict["Remark"] : 
                                    dict.ContainsKey("remark") ? dict["remark"] : "").ToString() ?? "";
                    
                    DateTime date = DateTime.UtcNow;
                    try {
                        if (dict.ContainsKey("Date")) date = Convert.ToDateTime(dict["Date"]);
                        else if (dict.ContainsKey("date")) date = Convert.ToDateTime(dict["date"]);
                    } catch { /* fallback to UtcNow */ }

                    var st = studentList.FirstOrDefault(s => s.Sid.ToString() == sId || s.Id == sId);
                    
                    return new StudentAttedenceView
                    {
                        Id = dict.ContainsKey("_id") ? dict["_id"].ToString() : "",
                        StudentId = sId,
                        Fullname = st != null ? $"{st.Fname} {st.Lname}".Trim() : "Unknown Student",
                        ImageUrl = st?.ImageUrl ?? "",
                        FacultyId = facultyId,
                        Status = status,
                        Remark = remark,
                        Date = date.ToLocalTime()
                    };
                }).OrderByDescending(x => x.Date).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "History fetch failed", error = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllStudents() => Ok(await _student.Find(_ => true).ToListAsync());

        [HttpGet("{facultyId}/students")]
        public async Task<IActionResult> GetStudentsByFacultyId(string facultyId)
        {
            try
            {
                var bsonFilter = Builders<BsonDocument>.Filter.Eq("Faculty_Id", facultyId);

                var facultyCol = _student.Database.GetCollection<BsonDocument>("faculties");
                if (ObjectId.TryParse(facultyId, out ObjectId oid))
                {
                    var fDoc = await facultyCol.Find(Builders<BsonDocument>.Filter.Eq("_id", oid)).FirstOrDefaultAsync();
                    if (fDoc != null && fDoc.Contains("fid"))
                    {
                        if (fDoc["fid"].IsInt32 || fDoc["fid"].IsInt64)
                        {
                            long fidVal = fDoc["fid"].IsInt32 ? fDoc["fid"].AsInt32 : fDoc["fid"].AsInt64;
                            bsonFilter = Builders<BsonDocument>.Filter.Or(
                                bsonFilter,
                                Builders<BsonDocument>.Filter.Eq("Faculty_Id", fidVal.ToString()),
                                Builders<BsonDocument>.Filter.Eq("Faculty_Id", fidVal)
                            );
                        }
                    }
                }
                else if (long.TryParse(facultyId, out long fidNumeric))
                {
                    bsonFilter = Builders<BsonDocument>.Filter.Or(
                        bsonFilter,
                        Builders<BsonDocument>.Filter.Eq("Faculty_Id", fidNumeric.ToString()),
                        Builders<BsonDocument>.Filter.Eq("Faculty_Id", fidNumeric)
                    );
                }

                var students = await _student.Database.GetCollection<BsonDocument>("StudentTbl").Find(bsonFilter).ToListAsync();
                return Ok(students.Select(s => MongoDB.Bson.Serialization.BsonSerializer.Deserialize<Student>(s)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching students", error = ex.Message });
            }
        }

        private int GetNextSid()
        {
            var filter = Builders<Counter>.Filter.Eq(c => c.Id, "Sid");
            var update = Builders<Counter>.Update.Inc(c => c.Seq, 1);
            var options = new FindOneAndUpdateOptions<Counter> { IsUpsert = true, ReturnDocument = ReturnDocument.After };
            var counter = _counters.FindOneAndUpdate(filter, update, options);
            return counter.Seq;
        }

        [HttpPost("add")]
        public IActionResult AddStudent([FromBody] Student student)
        {
            student.Sid = GetNextSid();
            _student.InsertOne(student);
            return Ok(new { message = "Added", student });
        }

        // 🚀 BULLETPROOF ADDITIONS FOR VIEW, EDIT, AND DELETE

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudent(string id)
        {
            try
            {
                var filterBuilder = Builders<Student>.Filter;
                FilterDefinition<Student> filter;

                if (ObjectId.TryParse(id, out _))
                {
                    filter = filterBuilder.Eq(s => s.Id, id);
                }
                else if (int.TryParse(id, out int numericSid))
                {
                    filter = filterBuilder.Eq(s => s.Sid, numericSid);
                }
                else
                {
                    return BadRequest(new { message = "Invalid API ID format." });
                }

                var student = await _student.Find(filter).FirstOrDefaultAsync();
                if (student == null) return NotFound(new { message = "Student not found." });
                return Ok(student);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(string id, [FromBody] Student updated)
        {
            try
            {
                var filterBuilder = Builders<Student>.Filter;
                FilterDefinition<Student> filter;

                if (ObjectId.TryParse(id, out _))
                {
                    filter = filterBuilder.Eq(s => s.Id, id);
                }
                else if (int.TryParse(id, out int numericSid))
                {
                    filter = filterBuilder.Eq(s => s.Sid, numericSid);
                }
                else
                {
                    return BadRequest(new { message = "Invalid API ID format." });
                }

                var existingStudent = await _student.Find(filter).FirstOrDefaultAsync();
                if (existingStudent == null) return NotFound(new { message = "Student not found." });

                // CRITICAL FIX: Lock in the immutable _id and Sid so ReplaceOneAsync doesn't crash!
                updated.Id = existingStudent.Id; 
                updated.Sid = existingStudent.Sid;
                
                var result = await _student.ReplaceOneAsync(filter, updated);
                
                return Ok(new { message = "Student updated successfully." });
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            try
            {
                var filterBuilder = Builders<Student>.Filter;
                FilterDefinition<Student> filter;

                if (ObjectId.TryParse(id, out _))
                {
                    filter = filterBuilder.Eq(s => s.Id, id);
                }
                else if (int.TryParse(id, out int numericSid))
                {
                    filter = filterBuilder.Eq(s => s.Sid, numericSid);
                }
                else
                {
                    return BadRequest(new { message = "Invalid API ID format." });
                }

                var result = await _student.DeleteOneAsync(filter);
                if (result.DeletedCount == 0) return NotFound(new { message = "Student not found." });

                return Ok(new { message = "Student deleted successfully." });
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("my-attendance/{sid}")]
        public async Task<IActionResult> GetMyAttendance(string sid)
        {
            try
            {
                var filterBuilder = Builders<StudentAttendance>.Filter;
                var filter = filterBuilder.Eq(a => a.StudentIdObj, sid);

                if (ObjectId.TryParse(sid, out ObjectId oid))
                {
                    filter = filterBuilder.Or(filter, filterBuilder.Eq(a => a.StudentIdObj, oid));
                }

                if (long.TryParse(sid, out long numericSid))
                {
                    filter = filterBuilder.Or(
                        filter, 
                        filterBuilder.Eq(a => a.StudentIdObj, (int)numericSid),
                        filterBuilder.Eq(a => a.StudentIdObj, numericSid)
                    );
                }

                var records = await _attendanceCollection.Find(filter).ToListAsync();
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching attendance", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Student student)
        {
            try
            {
                student.Sid = GetNextSid();
                student.CreatedAt = DateTime.UtcNow;
                await _student.InsertOneAsync(student);
                return Ok(new { message = "Registration Successful", student });
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("my-results/{sid}")]
        public async Task<IActionResult> GetMyResults(string sid)
        {
            try
            {
                var filter = Builders<ExamMark>.Filter.Eq(m => m.StudentId, sid);
                var results = await _examMarks.Find(filter).ToListAsync();
                return Ok(results);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("assignments/{dept}")]
        public async Task<IActionResult> GetAssignmentsByDept(string dept)
        {
            try
            {
                var collection = _student.Database.GetCollection<Assignment>("Assignments");
                var records = await collection.Find(a => a.Department == dept).SortByDescending(a => a.PostedOn).ToListAsync();
                return Ok(records);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("materials/{dept}")]
        public async Task<IActionResult> GetMaterialsByDept(string dept)
        {
            try
            {
                var collection = _student.Database.GetCollection<StudyMaterial>("StudyMaterials");
                var records = await collection.Find(m => m.Department == dept).SortByDescending(m => m.PostedOn).ToListAsync();
                return Ok(records);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("profile/{sid}")]
        public async Task<IActionResult> GetProfile(string sid)
        {
            try
            {
                var filter = Builders<Student>.Filter.Eq(s => s.Id, sid);
                if (int.TryParse(sid, out int numericSid)) filter = Builders<Student>.Filter.Eq(s => s.Sid, numericSid);
                
                var student = await _student.Find(filter).FirstOrDefaultAsync();
                return Ok(student);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPost("suggestion")]
        public async Task<IActionResult> SubmitSuggestion([FromBody] F_Suggestion suggestion)
        {
            try
            {
                // Ensure SuggestionId is unique using a specialized counter
                var filter = Builders<Counter>.Filter.Eq(c => c.Id, "SuggestionId");
                var update = Builders<Counter>.Update.Inc(c => c.Seq, 1);
                var options = new FindOneAndUpdateOptions<Counter> { IsUpsert = true, ReturnDocument = ReturnDocument.After };
                var counter = await _counters.FindOneAndUpdateAsync(filter, update, options);

                suggestion.SuggestionId = counter.Seq;
                suggestion.PostedAt = DateTime.UtcNow;
                suggestion.Status = "Pending";
                suggestion.Reply = "Awaiting feedback...";
                
                await _suggestions.InsertOneAsync(suggestion);
                return Ok(new { message = "✅ Suggestion submitted successfully!" });
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }

    public class Counter
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public string Id { get; set; } = string.Empty;
        public int Seq { get; set; }
    }
}