namespace StudentAttendance.src.StudentAttendance.Domain.Auth
{
    public interface IJwtTokenProvider
    {
        string GenerateToken(JwtUserDescriptor user);
    }
}