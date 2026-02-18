using Microsoft.AspNetCore.Mvc;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Attendance;
using StudentAttendance.src.StudentAttendance.Application.Interfaces;

namespace StudentAttendance.src.StudentAttendance.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AttendanceController : ControllerBase
	{
		private readonly IAttendanceService _attendanceService;

		public AttendanceController(IAttendanceService attendanceService)
		{
			_attendanceService = attendanceService;
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

		// ETUDIANT: Consulter ses absences
		// GET /api/attendance/students/{studentId}/absences
		[HttpGet("students/{studentId}/absences")]
		public async Task<IActionResult> GetStudentAbsences([FromRoute] string studentId)
		{
			var result = await _attendanceService.GetMyAbsencesAsync(studentId);
			return Ok(result);
		}
	}
}
