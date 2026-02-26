using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentAttendance.src.StudentAttendance.Domain.Auth;
using StudentAttendance.src.StudentAttendance.Infrastructure.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Providers
{
    public class JwtTokenProvider : IJwtTokenProvider
    {

        private readonly JwtOptions _opt;
        public JwtTokenProvider(IOptions<JwtOptions> opt)
        {
            _opt = opt.Value;
        }

        public string GenerateToken(JwtUserDescriptor user)
        {
            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, user.UserId),
                new (JwtRegisteredClaimNames.Email, user.Email),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach(var kv in user.Claims)
                claims.Add(new Claim(kv.Key, kv.Value));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_opt.ExpMinuts),
                signingCredentials: creds
            );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
