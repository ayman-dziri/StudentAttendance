using System.ComponentModel.DataAnnotations;

namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Group
{
    public class UpdateGroupDto
    {
        [Required(ErrorMessage ="Le label est obligatoire")]
        [StringLength(maximumLength:20, MinimumLength = 2, ErrorMessage = "Le lable doit contenir entre 2 et 20 caractères")]   
        public string Label { get; set; } = string.Empty;
    }
}
