using System.ComponentModel.DataAnnotations;

namespace StudentAttendanceV2.src.StudentAttendance.Application.DTOs.Auth.Requests
{
    public class ChangePasswordRequest
    {   
        [Required]
        public string OldPassword {get; set;} = string.Empty ;


        [Required]
        [StringLength(100 , MinimumLength = 6)]
        public string NewPassword {get; set;} = string.Empty ;
    }
}