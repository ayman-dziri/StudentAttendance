namespace StudentAttendance.src.StudentAttendance.Application.Exceptions
{
    public class AccountDisabledException : Exception
    {
        public AccountDisabledException()
            : base("Compte desactive")
        { }
    }
}
