using StudentAttendance.src.StudentAttendance.Domain.Enums;
using System.CodeDom.Compiler;

namespace StudentAttendance.src.StudentAttendance.Domain.Entities
{
    public class User
    {

        public string Id { get; set; } = null!;
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public DateOnly BirthDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Role Role { get; set; }

        public bool IsActive { get; set; } = true;

        public string? GroupId { get; set; }


        public void GenerateEmail() // generer un email automatiquement en concatenant le nom + .prenom + .@Winity-artner.com
        {
            var domain = "@Winity-Partner.com";
            var UpperLastname = LastName.ToUpper();
            Email = $"{FirstName}.{UpperLastname}.{domain}";
        }
    }
}
