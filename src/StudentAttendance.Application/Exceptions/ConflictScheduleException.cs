namespace StudentAttendance.src.StudentAttendance.Application.Exceptions;

/// <summary>
/// Exception levée quand un conflit horaire est détecté
/// </summary>
public class ConflictScheduleException : Exception
{
    public ConflictScheduleException(string message)
        : base(message)
    {
    }
}