

namespace StudentAttendance.src.StudentAttendance.Application.DTOs.absence;

    public class UpdateAbsenceStatusRequest
    {
        public string AbsenceId { get; set; } =null!;
        public string Status { get; set; }=null!;
    }
