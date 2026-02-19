namespace StudentAttendance.src.StudentAttendance.Application.Exceptions;

/// <summary>
/// Exception levée quand une entité n'est pas trouvée
/// </summary>
public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string message)
        : base(message)
    {
    }
}