using Moq;
using Microsoft.Extensions.Logging;
using StudentAttendance.src.StudentAttendance.Application.Exceptions;
using StudentAttendance.src.StudentAttendance.Application.Services;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Enums;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Application.DTOs.absence;
using Xunit;

namespace StudentAttendance.Application.Tests.Services;

public class AbsenceServiceTest
{
    private readonly Mock<IAbsenceRepository> _mockRepository;
    private readonly Mock<ILogger<AbsenceService>> _mockLogger;
    private readonly AbsenceService _service;

    public AbsenceServiceTest()
    {
        _mockRepository = new Mock<IAbsenceRepository>();
        _mockLogger = new Mock<ILogger<AbsenceService>>();
        _service = new AbsenceService(_mockRepository.Object, _mockLogger.Object);
    }

    // ==================== CreateAbsencesForSessionAsync ====================

    [Fact]
    public async Task CreateAbsencesForSession_WithStudents_ShouldInsertAbsencesWithPresentStatus()
    {
        // Arrange
        var sessionId = "000000000000000000000099";
        var studentIds = new List<string> { "000000000000000000000001", "000000000000000000000002" };

        // Act
        await _service.CreateAbsencesForSessionAsync(sessionId, studentIds);

        // Assert
        _mockRepository.Verify(r => r.InsertManyAsync(
            It.Is<List<Absence>>(absences =>
                absences.Count == 2 &&
                absences.All(a => a.Status == StatusPresence.PRESENT) &&
                absences.All(a => a.SessionId == sessionId) &&
                absences.All(a => a.JustificationDate == null)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAbsencesForSession_WithEmptyList_ShouldNotCallRepository()
    {
        // Arrange
        var studentIds = new List<string>();

        // Act
        await _service.CreateAbsencesForSessionAsync("session1", studentIds);

        // Assert
        _mockRepository.Verify(r => r.InsertManyAsync(
            It.IsAny<List<Absence>>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ==================== GetAbsencesBySessionAsync ====================

    [Fact]
    public async Task GetAbsencesBySession_ShouldReturnAbsences()
    {
        // Arrange
        var sessionId = "000000000000000000000099";
        var expectedAbsences = new List<Absence>
        {
            new() { Id = "1", StudentId = "s1", SessionId = sessionId, Status = StatusPresence.PRESENT },
            new() { Id = "2", StudentId = "s2", SessionId = sessionId, Status = StatusPresence.ABSENT }
        };

        _mockRepository
            .Setup(r => r.GetBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAbsences);

        // Act
        var result = await _service.GetAbsencesBySessionAsync(sessionId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(expectedAbsences, result);
    }

    // ==================== JustifyAbsenceAsync ====================

    [Fact]
    public async Task JustifyAbsence_WithAbsentStatus_ShouldUpdateToJustified()
    {
        // Arrange
        var absenceId = "000000000000000000000001";
        var absence = new Absence
        {
            Id = absenceId,
            StudentId = "s1",
            SessionId = "session1",
            Status = StatusPresence.ABSENT,
            JustificationDate = null
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(absenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(absence);

        // Act
        await _service.JustifyAbsenceAsync(absenceId);

        // Assert
        Assert.Equal(StatusPresence.JUSTIFIED, absence.Status);
        Assert.NotNull(absence.JustificationDate);
        _mockRepository.Verify(r => r.UpdateAsync(absence, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task JustifyAbsence_WithLateStatus_ShouldUpdateToJustified()
    {
        // Arrange
        var absenceId = "000000000000000000000001";
        var absence = new Absence
        {
            Id = absenceId,
            StudentId = "s1",
            SessionId = "session1",
            Status = StatusPresence.LATE,
            JustificationDate = null
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(absenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(absence);

        // Act
        await _service.JustifyAbsenceAsync(absenceId);

        // Assert
        Assert.Equal(StatusPresence.JUSTIFIED, absence.Status);
        Assert.NotNull(absence.JustificationDate);
    }

    [Fact]
    public async Task JustifyAbsence_WithNonExistentId_ShouldThrowAbsenceNotFoundException()
    {
        // Arrange
        var absenceId = "000000000000000000000099";

        _mockRepository
            .Setup(r => r.GetByIdAsync(absenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Absence?)null);

        // Act & Assert
        await Assert.ThrowsAsync<AbsenceNotFoundException>(
            () => _service.JustifyAbsenceAsync(absenceId));
    }

    [Fact]
    public async Task JustifyAbsence_WithAlreadyJustified_ShouldThrowAbsenceAlreadyJustifiedException()
    {
        // Arrange
        var absenceId = "000000000000000000000001";
        var absence = new Absence
        {
            Id = absenceId,
            Status = StatusPresence.JUSTIFIED,
            JustificationDate = DateTime.UtcNow
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(absenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(absence);

        // Act & Assert
        await Assert.ThrowsAsync<AbsenceAlreadyJustifiedException>(
            () => _service.JustifyAbsenceAsync(absenceId));
    }

    [Fact]
    public async Task JustifyAbsence_WithPresentStatus_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var absenceId = "000000000000000000000001";
        var absence = new Absence
        {
            Id = absenceId,
            Status = StatusPresence.PRESENT
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(absenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(absence);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.JustifyAbsenceAsync(absenceId));
    }
  // ==================== UpdateAbsenceStatusAsync ====================

    [Fact]
    public async Task UpdateAbsenceStatus_FromPresentToAbsent_ShouldUpdate()
    {
        // Arrange
        var absenceId = "000000000000000000000001";
        var absence = new Absence
        {
            Id = absenceId,
            StudentId = "s1",
            SessionId = "session1",
            Status = StatusPresence.PRESENT
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(absenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(absence);

        // Act
        await _service.UpdateAbsenceStatusAsync(absenceId, "ABSENT");

        // Assert
        Assert.Equal(StatusPresence.ABSENT, absence.Status);
        _mockRepository.Verify(r => r.UpdateAsync(absence, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAbsenceStatus_FromPresentToLate_ShouldUpdate()
    {
        // Arrange
        var absenceId = "000000000000000000000001";
        var absence = new Absence
        {
            Id = absenceId,
            Status = StatusPresence.PRESENT
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(absenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(absence);

        // Act
        await _service.UpdateAbsenceStatusAsync(absenceId, "LATE");

        // Assert
        Assert.Equal(StatusPresence.LATE, absence.Status);
    }

    [Fact]
    public async Task UpdateAbsenceStatus_FromAbsentToPresent_ShouldUpdate()
    {
        // Arrange
        var absenceId = "000000000000000000000001";
        var absence = new Absence
        {
            Id = absenceId,
            Status = StatusPresence.ABSENT
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(absenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(absence);

        // Act
        await _service.UpdateAbsenceStatusAsync(absenceId, "PRESENT");

        // Assert
        Assert.Equal(StatusPresence.PRESENT, absence.Status);
    }

    [Fact]
    public async Task UpdateAbsenceStatus_WhenJustified_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var absenceId = "000000000000000000000001";
        var absence = new Absence
        {
            Id = absenceId,
            Status = StatusPresence.JUSTIFIED
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(absenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(absence);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UpdateAbsenceStatusAsync(absenceId, "ABSENT"));
    }

    [Fact]
    public async Task UpdateAbsenceStatus_WithSameStatus_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var absenceId = "000000000000000000000001";
        var absence = new Absence
        {
            Id = absenceId,
            Status = StatusPresence.ABSENT
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(absenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(absence);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UpdateAbsenceStatusAsync(absenceId, "ABSENT"));
    }

    [Fact]
    public async Task UpdateAbsenceStatus_WithInvalidStatus_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var absenceId = "000000000000000000000001";
        var absence = new Absence
        {
            Id = absenceId,
            Status = StatusPresence.PRESENT
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(absenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(absence);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UpdateAbsenceStatusAsync(absenceId, "EXCUSED"));
    }

    [Fact]
    public async Task UpdateAbsenceStatus_WithNonExistentId_ShouldThrowAbsenceNotFoundException()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetByIdAsync("unknown", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Absence?)null);

        // Act & Assert
        await Assert.ThrowsAsync<AbsenceNotFoundException>(
            () => _service.UpdateAbsenceStatusAsync("unknown", "ABSENT"));
    }

    // ==================== UpdateAbsencesBulkAsync ====================

    [Fact]
    public async Task UpdateAbsencesBulk_WithMultipleUpdates_ShouldUpdateAll()
    {
        // Arrange
        var absence1 = new Absence { Id = "id1", Status = StatusPresence.PRESENT };
        var absence2 = new Absence { Id = "id2", Status = StatusPresence.PRESENT };

        _mockRepository.Setup(r => r.GetByIdAsync("id1", It.IsAny<CancellationToken>())).ReturnsAsync(absence1);
        _mockRepository.Setup(r => r.GetByIdAsync("id2", It.IsAny<CancellationToken>())).ReturnsAsync(absence2);

        var updates = new List<UpdateAbsenceStatusRequest>
        {
            new() { AbsenceId = "id1", Status = "ABSENT" },
            new() { AbsenceId = "id2", Status = "LATE" }
        };

        // Act
        await _service.UpdateAbsencesBulkAsync(updates);

        // Assert
        Assert.Equal(StatusPresence.ABSENT, absence1.Status);
        Assert.Equal(StatusPresence.LATE, absence2.Status);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Absence>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task UpdateAbsencesBulk_WithEmptyList_ShouldNotCallRepository()
    {
        // Act
        await _service.UpdateAbsencesBulkAsync(new List<UpdateAbsenceStatusRequest>());

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Absence>(), It.IsAny<CancellationToken>()), Times.Never);
    }

}