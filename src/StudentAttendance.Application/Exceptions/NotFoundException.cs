namespace StudentAttendance.src.StudentAttendance.Application.Exceptions
{
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
