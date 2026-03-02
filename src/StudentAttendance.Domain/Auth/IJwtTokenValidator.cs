using System.Security.Claims;

namespace StudentAttendance.src.StudentAttendance.Domain.Auth
{
    public interface IJwtTokenValidator
    {
        ClaimsPrincipal? Validate(string token);
    }
}
