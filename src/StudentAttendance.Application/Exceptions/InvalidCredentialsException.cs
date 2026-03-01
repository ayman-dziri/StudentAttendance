namespace StudentAttendance.src.StudentAttendance.Application.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() 
            : base("Identifiants incorrects")
        { }
    }
}
