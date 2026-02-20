using System.ComponentModel.DataAnnotations;

namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Group
{
    public class CreateGroupDto
    {
        [Required(ErrorMessage = "Le label est obligatoire")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Le label doit contenir entre 2 et 20 caractères")]
        public string Label { get; set; } = string.Empty;
    }
}
