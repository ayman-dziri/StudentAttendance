using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Infrastructure.Documents;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Mappers
{
    public class SessionMapper
    {

        public static SessionDocument ToDocument(Session u) => new() // Transformer une entité Domain (Session) en objet Mongo (SessionDocument)

        {
            //copier simplement les propriétés.
            Id = u.Id,
            StartTime = u.StartTime,
            EndTime = u.EndTime,
            TeacherId = u.TeacherId,
            Group = u.Group,
            IsValidated = u.IsValidated
        };


        //l'inverse
        public static Session ToDomain(SessionDocument d) => new() //Quand tu fais un Find dans Mongo, tu récupères un SessionDocument

        {
            Id = d.Id,
            StartTime = d.StartTime,
            EndTime = d.EndTime,
            TeacherId = d.TeacherId,
            Group = d.Group,
            IsValidated = d.IsValidated
        };
    }
}
