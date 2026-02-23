using StudentAttendance.src.StudentAttendance.Domain.Enums;

namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Auth.Requests
{
    public record RegisterRequest(string FirstName , string LastName , string Email , string Password , Role Role);
   
}
