using System.ComponentModel.DataAnnotations;

namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Auth
{
    public record LoginRequestDto(
        [Required]
        string Email,
        [Required]
        string Password
    );   
}