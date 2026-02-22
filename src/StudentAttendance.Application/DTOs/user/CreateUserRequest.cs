using StudentAttendance.src.StudentAttendance.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace StudentAttendance.src.StudentAttendance.Application.DTOs.user
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "firstname is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "lastname is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        public DateOnly BirthDate { get; set; }

        public Role Role { get; set; }

        public string? GroupId { get; set; }
    }
}
