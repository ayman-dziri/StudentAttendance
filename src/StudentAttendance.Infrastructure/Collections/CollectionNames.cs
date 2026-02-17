namespace StudentAttendance.src.StudentAttendance.Infrastructure.Collections;

/// <summary>
/// Noms des collections MongoDB centralisés — évite les magic strings
/// </summary>
public static class CollectionNames
{
    public const string Users = "users";
    public const string Groups = "groups";
    public const string Sessions = "sessions";
    public const string Absences = "absences";
}