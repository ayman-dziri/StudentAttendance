namespace StudentAttendance.src.StudentAttendance.Application.Exceptions
{
    public class DuplicateGroupException : Exception
    {   
        public DuplicateGroupException(string label)
            : base($"un groupe avec le label '{label}' existe déjà")
        {
        }
    }
}
