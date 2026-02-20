using Microsoft.AspNetCore.Mvc;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Attendance;
using StudentAttendance.src.StudentAttendance.Application.Interfaces;
using StudentAttendance.src.StudentAttendance.Domain.Enums;
using StudentAttendance.src.StudentAttendance.Domain.IRepositories;

namespace StudentAttendance.src.StudentAttendance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly ISessionRepository _sessions;

        public AttendanceController(IAttendanceService attendanceService, ISessionRepository sessions)
        {
            _attendanceService = attendanceService;
            _sessions = sessions;
        }

        // PROF: Valider une séance + marquer absences
        // POST /api/attendance/teachers/{teacherId}/sessions/{sessionId}/validate
        [HttpPost("teachers/{teacherId}/sessions/{sessionId}/validate")]
        public async Task<IActionResult> ValidateSessionAndMarkAbsences(
            [FromRoute] string teacherId,
            [FromRoute] string sessionId,
            [FromBody] MarkAbsencesRequest request)
        {
            await _attendanceService.ValidateAndMarkAsync(teacherId, sessionId, request);
            return NoContent(); // 204
        }

        // ETUDIANT: Consulter ses absences (ancienne route)
        // GET /api/attendance/students/{studentId}/absences
        [HttpGet("students/{studentId}/absences")]
        public async Task<IActionResult> GetStudentAbsences([FromRoute] string studentId)
        {
            var userId = User.FindFirst("id")?.Value ?? User.FindFirst("sub")?.Value;

            //  DEV ONLY: Swagger sans token
            if (string.IsNullOrWhiteSpace(userId) && Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                userId = studentId;

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Missing user id claim.");

            if (studentId != userId)
                return Forbid("You can only access your own absences.");

            var result = await _attendanceService.GetMyAbsencesAsync(studentId);
            return Ok(result);
        }
        // ETUDIANT: route recommandée (ME) pour ne pas passer l'id en paramètre et éviter les problèmes d'autorisation 
        // GET /api/attendance/me/absences
        [HttpGet("me/absences")]
        public async Task<IActionResult> GetMyAbsences()
        {
            var userId = User.FindFirst("id")?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Missing user id claim.");

            var roleStr = User.FindFirst("role")?.Value;
            if (string.IsNullOrWhiteSpace(roleStr))
                return Unauthorized("Missing role claim.");

            if (!Enum.TryParse<Role>(roleStr, ignoreCase: true, out var role))
                return Unauthorized("Invalid role claim.");

            if (role != Role.STUDENT)
                return Forbid("Only STUDENT can access this endpoint.");

            var result = await _attendanceService.GetMyAbsencesAsync(userId);
            return Ok(result);
        }
    }
}