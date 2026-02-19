// Correct
namespace StudentAttendance.src.StudentAttendance.Application.Exceptions;
//<summary>
/// Exception levée lorsque l'absence n'est pas trouvée
/// </summary>
public class AbsenceNotFoundException : Exception
{
    public AbsenceNotFoundException(string absenceId)
        : base($"Absence avec ID '{absenceId}' est introuvable.")
    {
    }
}
