using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Requests;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Application.Services;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Infrastructure.Repositories;
using System.ComponentModel;

using Xunit;

namespace StudentAttendance.test.StudentAttendance.Application.Tests.Services;

public class SessionsServiceTest
{

    private readonly Mock<ISessionsRepository> _sessionsRepositoryMock = new();

    private readonly Mock<IAbsenceService> _absenceService = new();

    private readonly Mock<ILogger<SessionsService>> _mockLogger = new();

    private readonly Mock<IValidator<CreateSessionRequest>> _createValidator = new();
    private readonly Mock<IValidator<UpdateSessionRequest>> _updateValidator = new();

    private readonly Mock<ISessionConflictValidator> _sessionConflictValidator = new();




    private SessionsService CreateSut() //System Under Test
    =>
        //Creer l'objet qu'on est en train de tester
        new SessionsService(
            _absenceService.Object,
            _sessionsRepositoryMock.Object,
            _mockLogger.Object,
            _createValidator.Object,
            _updateValidator.Object,
            _sessionConflictValidator.Object

            );
    

    //Get All Sessions
    [Fact(DisplayName = "Get All Sessions Should return Mapped Response")]

    public async Task GetAllSessionsAsync_ShouldReturnMappedResponse()
    {
        _sessionsRepositoryMock
            .Setup(r => r.GetAllSessionsAsync())
            .ReturnsAsync(new List<Session>
            {
                new Session {
                    Id = "s1" ,
                    TeacherId = "t1" ,
                    Group = "G1" ,
                    StartTime = DateTime.UtcNow ,
                    EndTime = DateTime.UtcNow.AddHours(1),
                    Statut = false 
                } ,
                 new Session {
                    Id = "s2" ,
                    TeacherId = "t2" ,
                    Group = "G2" ,
                    StartTime = DateTime.UtcNow ,
                    EndTime = DateTime.UtcNow.AddHours(2) ,
                    Statut = false
                }
            });

        var sut = CreateSut();


        var result = await sut.GetAllSessionsAsync();


        result.Should().HaveCount(2);
        result.Select(x => x.Id).Should().Contain(new[] { "s1", "s2" });



    }


    [Fact]
    public async Task GetSessionsByIdAsync_WhenNotFound_ShouldReturnNull()
    {
        
        _sessionsRepositoryMock
            .Setup(r => r.GetSessionsByIdAsync("missing"))
            .ReturnsAsync((Session?)null);

        var sut = CreateSut();

        
        var result = await sut.GetSessionsByIdAsync("missing");

        
        result.Should().BeNull();
    }


    [Fact]
    public async Task GetSessionsByIdAsync_WhenFounded_ShouldReturnMappedResponse()
    {
        _sessionsRepositoryMock.Setup(r => r.GetSessionsByIdAsync("s1"))
            .ReturnsAsync(new Session
            {
                Id = "s1",
                TeacherId = "t1",
                Group = "G1",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                Statut = false
            });


        var sut = CreateSut();

        var result = await sut.GetSessionsByIdAsync("s1");

        result.Should().NotBeNull();
        result.Id.Should().Be("s1");
    }




    [Fact]
    public async Task GetSessionsByGroupName_ShouldReturnMappedResponses()
    {
       
        _sessionsRepositoryMock
            .Setup(r => r.GetSessionsByGroupName("G1"))
            .ReturnsAsync(new List<Session>
            {
                    new Session { 
                        Id = "s1", 
                        TeacherId = "t1",
                        Group = "G1",
                        StartTime = DateTime.UtcNow,
                        EndTime = DateTime.UtcNow.AddHours(1),
                        Statut = false
                    }
            });

        var sut = CreateSut();

       
        var result = await sut.GetSessionsByGroupName("G1");

        
        result.Should().HaveCount(1);
        result[0].Group.Should().Be("G1");
    }

    

    [Fact]
    public async Task GetSessionsByTeacherIdAsync_ShouldReturnMappedResponses()
    {
       
        _sessionsRepositoryMock
            .Setup(r => r.GetSessionsByTeacherIdAsync("t1"))
            .ReturnsAsync(new List<Session>
            {
                    new Session { Id = "s1",
                        TeacherId = "t1",
                        Group = "G1",
                        StartTime = DateTime.UtcNow,
                        EndTime = DateTime.UtcNow.AddHours(1) ,
                        Statut = false
                    }
            });

        var sut = CreateSut();

       
        var result = await sut.GetSessionsByTeacherIdAsync("t1");

     
        result.Should().HaveCount(1);
        result[0].TeacherId.Should().Be("t1");
    }


    [Fact]
    public async Task CreateSessionsAsync_WhenInvalid_ShouldThrowValidationException()
    {
        
        var request = new CreateSessionRequest
        {
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(-1),
            TeacherId = "",
            Group = "",
            Statut = false

        };

        var failures = new List<ValidationFailure>
            {
                new ValidationFailure("TeacherId", "TeacherId is required")
            };

        _createValidator
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var sut = CreateSut();

        Func<Task> act = async () => await sut.CreateSessionsAsync(request);

        await act.Should().ThrowAsync<ValidationException>();
        _sessionsRepositoryMock.Verify(r => r.CreateSessionsAsync(It.IsAny<Session>()), Times.Never);
    }

    [Fact]
    public async Task CreateSessionsAsync_WhenValid_ShouldCreateSession_AndCreateAbsences()
    {
      
        var request = new CreateSessionRequest
        {
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(2),
            TeacherId = "t1",
            Group = "G1",
            Statut = false
        };

        _createValidator
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult()); // valid

        var created = new Session
        {
            Id = "session1",
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TeacherId = request.TeacherId,
            Group = request.Group,
            Statut = request.Statut
        };

        _sessionsRepositoryMock
            .Setup(r => r.CreateSessionsAsync(It.IsAny<Session>()))
            .ReturnsAsync(created);

        _sessionsRepositoryMock
            .Setup(r => r.GetStudentsBySessionIdAsync(created.Id))
            .ReturnsAsync(new List<User>
            {
                    new User { Id = "student1" },
                    new User { Id = "student2" }
            });

        var sut = CreateSut();

        var result = await sut.CreateSessionsAsync(request, CancellationToken.None);

        result.Id.Should().Be("session1");

        _absenceService.Verify(a =>
            a.CreateAbsencesForSessionAsync(
                "session1",
                It.Is<List<string>>(ids => ids.Count == 2 && ids.SequenceEqual(new[] { "student1", "student2" })),
                //It.Is<List<string>>(ids => ids.Count == 2 && ids.Contains("student1") && ids.Contains("student2")), 
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task UpdateSessionsAsync_WhenNotFound_ShouldReturnNull()
    {
       
        _sessionsRepositoryMock
            .Setup(r => r.GetSessionsByIdAsync("missing"))
            .ReturnsAsync((Session?)null);

        var sut = CreateSut();

        
        var result = await sut.UpdateSessionsAsync("missing", new UpdateSessionRequest());

        
        result.Should().BeNull();
    }

    // TODO: UpdateSessionsAsync_WhenInvalid_ShouldThrowValidationException
    // TODO: UpdateSessionsAsync_WhenValid_ShouldUpdateAndReturnResponse

  

    [Fact]
    public async Task DeleteSessionsAsync_WhenNotExists_ShouldReturnFalse()
    {
        
        _sessionsRepositoryMock
            .Setup(r => r.ExistsSessionAsync("missing"))
            .ReturnsAsync(false);

        var sut = CreateSut();

        
        var result = await sut.DeleteSessionsAsync("missing");

        
        result.Should().BeFalse();
    }

    
}


