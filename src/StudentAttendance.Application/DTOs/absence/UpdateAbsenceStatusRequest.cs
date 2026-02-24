
namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Absence
{
	public class UpdateAbsenceStatusRequest
	{
		public string AbsenceId { get; set; } = null!;
		public string Status { get; set; } = null!;
	}
}