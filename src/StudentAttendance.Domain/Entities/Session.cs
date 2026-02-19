namespace StudentAttendance.src.StudentAttendance.Domain.Entities
{
    public class Session
    {

        public string Id { get; set; } = null!;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string TeacherId { get; set; } = null!;

        public string Group { get; set; } = null!;
<<<<<<< HEAD
<<<<<<< HEAD

=======
>>>>>>> origin/feature/scrum-19-conflit-horaire
        public Boolean Statut { get; set; } = false;
=======

        public bool IsValidated { get; set; } = false; 

>>>>>>> origin/feature/scrum-12-attendance-validation
    }
}
