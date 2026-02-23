namespace StudentAttendance.src.StudentAttendance.Application.Exceptions
{
    public class GroupNotFoundException : Exception
    {
        public GroupNotFoundException(string message)
            : base(message)
        {
        }
        public static GroupNotFoundException ById(string id)
            => new GroupNotFoundException($"Groupe avec l'ID '{id}' non trouvé");

        public static GroupNotFoundException ByLabel(string label)
            => new GroupNotFoundException($"Groupe avec le label '{label}' non trouvé");
    }

}
