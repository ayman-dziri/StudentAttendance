using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Requests;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Response;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Infrastructure.Documents;

namespace StudentAttendance.src.StudentAttendance.Application.Mappers;

    public static class SessionMapper
    {
    //DTO created -> Domain entité
    public static Session ToEntity(CreateSessionRequest dto) => new() 

    {
        Id = Guid.NewGuid().ToString(), //copier simplement les propriétés.
        StartTime = dto.StartTime,
        EndTime = dto.EndTime,
        TeacherId = dto.TeacherId,
        Group = dto.Group,
        IsValidated = dto.IsValidated,
    };

    //DTO updated -> Domain entité
    public static void MapUpdate(UpdateSessionRequest request, Session session)
    {
        session.StartTime = request.StartTime;
        session.EndTime = request.EndTime;
        session.TeacherId = request.TeacherId;
        session.Group = request.Group;
        session.IsValidated = request.IsValidated;
    }


    //Entite -> Response DTO
    public static SessionResponse ToResponse(Session session) => new() 
    {
        Id = session.Id,
        StartTime = session.StartTime,
        EndTime = session.EndTime,
        TeacherId = session.TeacherId,
        Group = session.Group,
        IsValidated = session.IsValidated,
    };
}
    
