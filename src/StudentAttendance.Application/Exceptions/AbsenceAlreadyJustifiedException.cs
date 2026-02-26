namespace StudentAttendance.src.StudentAttendance.Application.Exceptions;
///<summary>
/// Exception levée lorsque l'absence est déjà justifiée
/// </summary>
public class AbsenceAlreadyJustifiedException : Exception
{
    public AbsenceAlreadyJustifiedException(string absenceId)
        : base($"Absence avec ID '{absenceId}' est déjà justifiée.")
    {
    }
}