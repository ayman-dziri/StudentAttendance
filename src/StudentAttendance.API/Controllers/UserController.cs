using Microsoft.AspNetCore.Mvc;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Infrastructure.Collections;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;

namespace StudentAttendance.src.StudentAttendance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly StudentAttendanceDbContext _db;

        public UserController(StudentAttendanceDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user, CancellationToken cancellationToken)
        {
            var collection = _db.GetCollection<User>(CollectionNames.Users);

            await collection.InsertOneAsync(user, cancellationToken: cancellationToken);

            return Ok(user);
        }
    }
}

