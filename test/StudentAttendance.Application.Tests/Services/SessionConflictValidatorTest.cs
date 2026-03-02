using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using StudentAttendance.src.StudentAttendance.Application.Exceptions;
using StudentAttendance.src.StudentAttendance.Application.Services;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using System.Globalization;

namespace StudentAttendance.Application.Tests.Services;

public class SessionConflictValidatorTest
{
    private readonly Mock<ISessionsRepository> _mockRepository;
    private readonly Mock<ILogger<SessionConflictValidator>> _mockLogger;
    private readonly SessionConflictValidator _validator;

    public SessionConflictValidatorTest()
    {
        _mockRepository = new Mock<ISessionsRepository>();
        _mockLogger = new Mock<ILogger<SessionConflictValidator>>();
        _validator = new SessionConflictValidator(_mockRepository.Object, _mockLogger.Object);
    }

    // ==================== Pas de conflit ====================

    [Fact]
    public async Task ValidateNoConflict_WithNoExistingSessions_ShouldNotThrow()
    {
        // Arrange
        var session = CreateSession("09:00", "10:00", "teacher1", "GroupA");

        _mockRepository.Setup(r => r.GetSessionsByGroupName("GroupA")).ReturnsAsync(new List<Session>());
        _mockRepository.Setup(r => r.GetSessionsByTeacherIdAsync("teacher1")).ReturnsAsync(new List<Session>());

        // Act & Assert
        await _validator.ValidateNoConflictAsync(session);
    }

    [Fact]
    public async Task ValidateNoConflict_WithNonOverlappingSessions_ShouldNotThrow()
    {
        // Arrange
        var newSession = CreateSession("09:00", "10:00", "teacher1", "GroupA");
        var existingSession = CreateSession("10:00", "11:00", "teacher1", "GroupA", "existing1");

        _mockRepository.Setup(r => r.GetSessionsByGroupName("GroupA")).ReturnsAsync(new List<Session> { existingSession });
        _mockRepository.Setup(r => r.GetSessionsByTeacherIdAsync("teacher1")).ReturnsAsync(new List<Session> { existingSession });

        // Act & Assert
        await _validator.ValidateNoConflictAsync(newSession);
    }

    // ==================== Conflit groupe ====================

    [Fact]
    public async Task ValidateNoConflict_WithGroupConflict_ShouldThrowConflictScheduleException()
    {
        // Arrange
        var newSession = CreateSession("09:00", "10:00", "teacher1", "GroupA");
        var existingSession = CreateSession("09:30", "10:30", "teacher2", "GroupA", "existing1");

        _mockRepository.Setup(r => r.GetSessionsByGroupName("GroupA")).ReturnsAsync(new List<Session> { existingSession });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictScheduleException>(
            () => _validator.ValidateNoConflictAsync(newSession));

        Assert.Contains("GroupA", exception.Message);
    }

    [Fact]
    public async Task ValidateNoConflict_WithSessionInsideExisting_ShouldThrowConflictScheduleException()
    {
        // Arrange : nouvelle séance entièrement dans une existante
        var newSession = CreateSession("09:30", "09:45", "teacher1", "GroupA");
        var existingSession = CreateSession("09:00", "10:00", "teacher2", "GroupA", "existing1");

        _mockRepository.Setup(r => r.GetSessionsByGroupName("GroupA")).ReturnsAsync(new List<Session> { existingSession });

        // Act & Assert
        await Assert.ThrowsAsync<ConflictScheduleException>(
            () => _validator.ValidateNoConflictAsync(newSession));
    }

    // ==================== Conflit professeur ====================

    [Fact]
    public async Task ValidateNoConflict_WithTeacherConflict_ShouldThrowConflictScheduleException()
    {
        // Arrange
        var newSession = CreateSession("09:00", "10:00", "teacher1", "GroupA");
        var existingSession = CreateSession("09:30", "10:30", "teacher1", "GroupB", "existing1");

        _mockRepository.Setup(r => r.GetSessionsByGroupName("GroupA")).ReturnsAsync(new List<Session>());
        _mockRepository.Setup(r => r.GetSessionsByTeacherIdAsync("teacher1")).ReturnsAsync(new List<Session> { existingSession });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictScheduleException>(
            () => _validator.ValidateNoConflictAsync(newSession));

        Assert.Contains("professeur", exception.Message);
    }

    // ==================== Modification (excludeSessionId) ====================

    [Fact]
    public async Task ValidateNoConflict_WhenModifyingSameSession_ShouldNotThrow()
    {
        // Arrange : modifier une séance ne doit pas entrer en conflit avec elle-même
        var session = CreateSession("09:00", "10:00", "teacher1", "GroupA", "session1");
        var existingSession = CreateSession("09:00", "10:00", "teacher1", "GroupA", "session1");

        _mockRepository.Setup(r => r.GetSessionsByGroupName("GroupA")).ReturnsAsync(new List<Session> { existingSession });
        _mockRepository.Setup(r => r.GetSessionsByTeacherIdAsync("teacher1")).ReturnsAsync(new List<Session> { existingSession });

        // Act & Assert : on exclut session1 de la vérification
        await _validator.ValidateNoConflictAsync(session, excludeSessionId: "session1");
    }

    [Fact]
    public async Task ValidateNoConflict_WhenModifyingWithOtherConflict_ShouldThrow()
    {
        // Arrange : modifier une séance mais conflit avec une AUTRE séance
        var session = CreateSession("09:00", "10:00", "teacher1", "GroupA", "session1");
        var otherSession = CreateSession("09:30", "10:30", "teacher1", "GroupA", "session2");

        _mockRepository.Setup(r => r.GetSessionsByGroupName("GroupA")).ReturnsAsync(new List<Session> { otherSession });

        // Act & Assert : session1 est exclue mais session2 est en conflit
        await Assert.ThrowsAsync<ConflictScheduleException>(
            () => _validator.ValidateNoConflictAsync(session, excludeSessionId: "session1"));
    }

    // ==================== Pas de chevauchement (limites) ====================

    [Fact]
    public async Task ValidateNoConflict_WhenSessionsEndAndStartAtSameTime_ShouldNotThrow()
    {
        // Arrange : une séance finit à 10h00 et l'autre commence à 10h00
        var newSession = CreateSession("10:00", "11:00", "teacher1", "GroupA");
        var existingSession = CreateSession("09:00", "10:00", "teacher1", "GroupA", "existing1");

        _mockRepository.Setup(r => r.GetSessionsByGroupName("GroupA")).ReturnsAsync(new List<Session> { existingSession });
        _mockRepository.Setup(r => r.GetSessionsByTeacherIdAsync("teacher1")).ReturnsAsync(new List<Session> { existingSession });

        // Act & Assert : pas de conflit, les séances sont bout à bout
        await _validator.ValidateNoConflictAsync(newSession);
    }

    // ==================== Helper ====================

    private static Session CreateSession(string startTime, string endTime, string teacherId, string group, string? id = null)
{
    var today = DateTime.Today;
    return new Session
    {
        Id = id ?? string.Empty,
        StartTime = today.Add(TimeSpan.Parse(startTime, CultureInfo.InvariantCulture)),
        EndTime = today.Add(TimeSpan.Parse(endTime, CultureInfo.InvariantCulture)),
        TeacherId = teacherId,
        Group = group
    };
}
}