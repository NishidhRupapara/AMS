using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models.Admin;
using WebApplication1.Models.Student;
using WebApplication1.Models.Faculty;
using WebApplication1.Models; 

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        private readonly IMongoCollection<AdminLogin> _adminlogin;
        private readonly IMongoCollection<StudentAttendance> _attendanceCollection;
        private readonly IMongoCollection<Student> _student;
        private readonly IMongoCollection<NoticeAdmin> _notices;
        private readonly IMongoCollection<F_Suggestion> _suggestions;
        private readonly IMongoCollection<BsonDocument> _departments; 
        private readonly IMongoCollection<Faculty> _faculty; 
        private readonly IMongoCollection<WebApplication1.Models.LeaveRequest> _leaveRequests;
        private readonly IMongoCollection<Assignment> _assignments;
        private readonly IMongoCollection<StudyMaterial> _studyMaterials;

        public AdminController(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            var db = client.GetDatabase(config["MongoDB:Database"] ?? "AMS");

            _adminlogin = db.GetCollection<AdminLogin>("Admin");
            _attendanceCollection = db.GetCollection<StudentAttendance>("StudentAt");
            _student = db.GetCollection<Student>("StudentTbl");
            _notices = db.GetCollection<NoticeAdmin>("AdminNotice");
            _suggestions = db.GetCollection<F_Suggestion>("F_Suggestion");
            _departments = db.GetCollection<BsonDocument>("Department");
            _faculty = db.GetCollection<Faculty>("faculties");
            _leaveRequests = db.GetCollection<WebApplication1.Models.LeaveRequest>("LeaveRequests");
            _assignments = db.GetCollection<Assignment>("Assignments");
            _studyMaterials = db.GetCollection<StudyMaterial>("StudyMaterials");
        }

        #region 1. Authentication
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLogin login)
        {
            if (login == null || string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
                return BadRequest(new { message = "Invalid login data" });

            var admin = await _adminlogin
                .Find(a => a.Username == login.Username && a.Password == login.Password)
                .FirstOrDefaultAsync();

            if (admin == null)
                return Unauthorized(new { message = "Username or password is incorrect" });

            return Ok(new { message = "Login successful", Aid = admin.Aid, Username = admin.Username });
        }
        #endregion

        #region 2. Attendance Management
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<StudentAttedenceView>>> GetAttendanceHistory()
        {
            var attendanceList = await _attendanceCollection.Find(_ => true).ToListAsync();
            var studentList = await _student.Find(_ => true).ToListAsync();
            var facultyList = await _faculty.Find(_ => true).ToListAsync();

            var result = attendanceList.Select(at => {
                var st = studentList.FirstOrDefault(s => s.Sid.ToString() == at.StudentId);
                var fac = facultyList.FirstOrDefault(f => f.Fid.ToString() == at.FacultyId || f.Id == at.FacultyId);
                
                return new StudentAttedenceView {
                    Id = at.Id,
                    StudentId = at.StudentId,
                    Fullname = st != null ? $"{st.Fname} {st.Lname}".Trim() : "Unknown Student",
                    FacultyId = at.FacultyId,
                    FacultyName = fac != null ? $"{fac.Fname} {fac.Lname}".Trim() : "Unknown Faculty",
                    Department = fac?.Department ?? "Unknown",
                    Status = at.Status,
                    Remark = at.Remark ?? "",
                    Date = at.Date.ToLocalTime(),
                    ImageUrl = st?.ImageUrl
                };
            }).OrderByDescending(x => x.Date).ToList();

            return Ok(result);
        }

        [HttpGet("history/{id}")]
        public async Task<IActionResult> GetAttendanceById(string id)
        {
            var record = await _attendanceCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
            if (record == null) return NotFound(new { message = "Record not found" });
            return Ok(record);
        }

        // 🚀 BULLETPROOF FIX: Using the new DTO to accept exact partial payloads from Angular
        [HttpPut("history/{id}")]
        public async Task<IActionResult> UpdateAttendance(string id, [FromBody] AttendanceUpdateDto updated)
        {
            try
            {
                var filter = Builders<StudentAttendance>.Filter.Eq(a => a.Id, id);
                var update = Builders<StudentAttendance>.Update
                    .Set(a => a.Status, updated.Status)
                    .Set(a => a.Remark, updated.Remark)
                    .Set(a => a.Date, updated.Date)
                    .Set(a => a.FacultyIdObj, updated.FacultyId);

                var result = await _attendanceCollection.UpdateOneAsync(filter, update);
                if (result.MatchedCount == 0) return NotFound(new { message = "Record not found" });
                
                return Ok(new { message = "Attendance updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Backend failed to process update", error = ex.Message });
            }
        }

        [HttpDelete("history/{id}")]
        public async Task<IActionResult> DeleteAttendance(string id)
        {
            var result = await _attendanceCollection.DeleteOneAsync(a => a.Id == id);
            if (result.DeletedCount == 0) return NotFound();
            return Ok(new { message = "Record deleted" });
        }
        #endregion

        #region 3. Notice Section
        [HttpPost("AddNotice")]
        public async Task<IActionResult> AddNotice([FromBody] NoticeAdmin notice)
        {
            var lastNotice = await _notices.Find(_ => true).SortByDescending(n => n.NoticeId).FirstOrDefaultAsync();
            notice.NoticeId = lastNotice != null ? lastNotice.NoticeId + 1 : 1;
            notice.CreatedAt = DateTime.UtcNow;
            await _notices.InsertOneAsync(notice);
            return Ok(new { message = "Notice posted", noticeId = notice.NoticeId });
        }

        [HttpGet("AllNoticeAdmin")]
        public async Task<IActionResult> GetAllNoticeAdmin() => Ok(await _notices.Find(_ => true).ToListAsync());

        [HttpGet("notices")]
        public async Task<IActionResult> GetNotices() => Ok(await _notices.Find(_ => true).SortByDescending(n => n.CreatedAt).ToListAsync());

        [HttpDelete("notice/{id}")]
        public async Task<IActionResult> DeleteNotice(int id)
        {
            await _notices.DeleteOneAsync(n => n.NoticeId == id);
            return Ok(new { message = "Notice deleted" });
        }
        #endregion

        #region 4. Department Management
        [HttpGet("view-departments")]
        public async Task<IActionResult> GetDepartments()
        {
            try
            {
                var list = await _departments.Find(_ => true).ToListAsync();
                
                var result = list.Select(d => new {
                    deptName = d.Contains("deptName") ? d["deptName"].ToString() : 
                               d.Contains("DepartmentName") ? d["DepartmentName"].ToString() : 
                               d.Contains("name") ? d["name"].ToString() : "Unknown"
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error", error = ex.Message });
            }
        }
        #endregion

        #region 5. Suggestions Section
        [HttpGet("AllSuggestionFaculty")]
        public async Task<IActionResult> GetAllSuggestions()
        {
            try
            {
                // Fix strict mapping crash for old records stored with integer FacultyIds
                var bsonCollection = _suggestions.Database.GetCollection<BsonDocument>("F_Suggestion");
                var list = await bsonCollection.Find(Builders<BsonDocument>.Filter.Empty).ToListAsync();
                
                var result = list.Select(doc => {
                    var dict = doc.ToDictionary();
                    return new {
                        id = dict.ContainsKey("_id") ? dict["_id"].ToString() : "",
                        suggestionId = dict.ContainsKey("SuggestionId") ? dict["SuggestionId"] : 0,
                        facultyId = dict.ContainsKey("FacultyId") ? dict["FacultyId"]?.ToString() : "",
                        target = dict.ContainsKey("Target") ? dict["Target"] : "Faculty",
                        targetName = dict.ContainsKey("TargetName") ? dict["TargetName"] : "Admin",
                        studentName = dict.ContainsKey("StudentName") ? dict["StudentName"] : "Student",
                        studentId = dict.ContainsKey("StudentId") ? dict["StudentId"]?.ToString() : "0",
                        title = dict.ContainsKey("Title") ? dict["Title"] : "",
                        message = dict.ContainsKey("Message") ? dict["Message"] : "",
                        postedAt = dict.ContainsKey("PostedAt") ? dict["PostedAt"] : DateTime.UtcNow,
                        status = dict.ContainsKey("Status") ? dict["Status"] : "Pending",
                        reply = dict.ContainsKey("Reply") ? dict["Reply"] : ""
                    };
                });
                return Ok(result);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPut("suggestion/{id}")]
        public async Task<IActionResult> UpdateSuggestion(string id, [FromBody] SuggestionUpdateDto updated)
        {
            if (updated == null) return BadRequest(new { message = "Invalid data payload." });
            if (!int.TryParse(id, out int numericId)) return BadRequest(new { message = "Invalid ID." });

            var filter = Builders<F_Suggestion>.Filter.Eq(s => s.SuggestionId, numericId);
            var update = Builders<F_Suggestion>.Update
                .Set(s => s.Status, updated.Status)
                .Set(s => s.Reply, updated.Reply);

            var result = await _suggestions.UpdateOneAsync(filter, update);
            if (result.MatchedCount == 0) return NotFound(new { message = "Suggestion not found." });

            return Ok(new { message = "Response saved successfully!" });
        }
        #endregion

        #region 6. Leave Request Management
        [HttpGet("all-leaves")]
        public async Task<IActionResult> GetAllLeaves()
        {
            var leaves = await _leaveRequests.Find(_ => true).SortByDescending(l => l.AppliedOn).ToListAsync();
            var faculties = await _faculty.Find(_ => true).ToListAsync();
            
            var result = leaves.Select(l => {
                var fac = faculties.FirstOrDefault(f => f.Fid.ToString() == l.FacultyId || f.Id == l.FacultyId);
                return new {
                    id = l.Id,
                    facultyId = l.FacultyId,
                    facultyName = fac != null ? $"{fac.Fname} {fac.Lname}".Trim() : "Unknown",
                    leaveType = l.LeaveType,
                    fromDate = l.FromDate,
                    toDate = l.ToDate,
                    reason = l.Reason,
                    status = l.Status,
                    appliedOn = l.AppliedOn,
                    adminRemark = l.AdminRemark
                };
            }).ToList();

            return Ok(result);
        }

        [HttpPut("leave/{id}")]
        public async Task<IActionResult> UpdateLeaveStatus(string id, [FromBody] LeaveUpdateDto updated)
        {
            var update = Builders<WebApplication1.Models.LeaveRequest>.Update
                .Set(l => l.Status, updated.Status)
                .Set(l => l.AdminRemark, updated.AdminRemark);
            await _leaveRequests.UpdateOneAsync(l => l.Id == id, update);
            return Ok(new { message = "Leave request updated!" });
        }
        #endregion

        #region 7. Academic Content Management (Assignments & Materials)
        [HttpGet("all-assignments")]
        public async Task<IActionResult> GetAllAssignments() => Ok(await _assignments.Find(_ => true).SortByDescending(a => a.PostedOn).ToListAsync());

        [HttpDelete("assignment/{id}")]
        public async Task<IActionResult> DeleteAssignment(string id)
        {
            await _assignments.DeleteOneAsync(a => a.Id == id);
            return Ok(new { message = "Assignment deleted" });
        }

        [HttpGet("all-materials")]
        public async Task<IActionResult> GetAllMaterials() => Ok(await _studyMaterials.Find(_ => true).SortByDescending(m => m.PostedOn).ToListAsync());

        [HttpDelete("material/{id}")]
        public async Task<IActionResult> DeleteMaterial(string id)
        {
            await _studyMaterials.DeleteOneAsync(m => m.Id == id);
            return Ok(new { message = "Material deleted" });
        }

        [HttpPost("post-material")]
        public async Task<IActionResult> PostMaterial([FromBody] StudyMaterial material)
        {
            material.PostedOn = DateTime.UtcNow;
            await _studyMaterials.InsertOneAsync(material);
            return Ok(new { message = "Material posted successfully" });
        }
        #endregion
    }

    // ==============================================================
    // 🛡️ DTO FOR ATTENDANCE UPDATE (Prevents 400 Validation Errors)
    // ==============================================================
    public class AttendanceUpdateDto
    {
        public string Status { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string FacultyId { get; set; } = string.Empty;
    }

    public class SuggestionUpdateDto
    {
        public string Status { get; set; } = string.Empty;
        public string Reply { get; set; } = string.Empty;
    }

    public class LeaveUpdateDto
    {
        public string Status { get; set; } = string.Empty;
        public string AdminRemark { get; set; } = string.Empty;
    }
}