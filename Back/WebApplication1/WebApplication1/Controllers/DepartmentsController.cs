using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IMongoCollection<Departments> _Departments;

        public DepartmentsController(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            var db = client.GetDatabase(config["MongoDB:Database"]);
            _Departments = db.GetCollection<Departments>("Department");
        }

        // GET: api/Departments/Dall
        [HttpGet("Dall")]
        public IActionResult GetAll()
        {
            var departments = _Departments.Find(_ => true).ToList();
            return Ok(departments);
        }

        // GET: api/Departments/{id}  --> View single department by Did
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var department = _Departments.Find(d => d.Did == id).FirstOrDefault();
            if (department == null)
                return NotFound(new { message = "Department not found" });
            return Ok(department);
        }

        // POST: api/Departments/AddDepartment
        [HttpPost("AddDepartment")]
        public IActionResult Create([FromBody] Departments department)
        {
            if (department == null || string.IsNullOrEmpty(department.DepartmentName))
                return BadRequest(new { message = "Department name is required" });

            // Get max Did
            var lastDept = _Departments
                .Find(_ => true)
                .SortByDescending(d => d.Did)
                .FirstOrDefault();

            int newDid = lastDept != null ? lastDept.Did + 1 : 1;
            department.Did = newDid;

            _Departments.InsertOne(department);

            return Ok(new { message = "Department added successfully", department });
        }

        // PUT: api/Departments/{id}  --> Update department by Did
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Departments updatedDepartment)
        {
            if (updatedDepartment == null || string.IsNullOrEmpty(updatedDepartment.DepartmentName))
                return BadRequest(new { message = "Department name is required" });

            var existingDept = _Departments.Find(d => d.Did == id).FirstOrDefault();
            if (existingDept == null)
                return NotFound(new { message = "Department not found" });

            var update = Builders<Departments>.Update
                .Set(d => d.DepartmentName, updatedDepartment.DepartmentName)
                .Set(d => d.ImageUrl, updatedDepartment.ImageUrl);

            _Departments.UpdateOne(d => d.Did == id, update);

            return Ok(new { message = "Department updated successfully", department = updatedDepartment });
        }

        // DELETE: api/Departments/{id}  --> Delete department by Did
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var department = _Departments.Find(d => d.Did == id).FirstOrDefault();
            if (department == null)
                return NotFound(new { message = "Department not found" });

            _Departments.DeleteOne(d => d.Did == id);

            return Ok(new { message = "Department deleted successfully" });
        }
    }
}
