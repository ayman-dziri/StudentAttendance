namespace StudentAttendance.src.StudentAttendance.Domain.Auth
{
    public sealed record JwtUserDescriptor(
        string UserId,
        string Email,
        IReadOnlyDictionary<string, string> Claims
    );
}
