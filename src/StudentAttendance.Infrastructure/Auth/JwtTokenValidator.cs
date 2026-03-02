using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentAttendance.src.StudentAttendance.Domain.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Auth
{
    public class JwtTokenValidator : IJwtTokenValidator
    {

        private readonly JwtOptions _opt;

        public JwtTokenValidator(IOptions<JwtOptions> opt)
        {
            _opt = opt.Value;
        }

        public ClaimsPrincipal? Validate(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _opt.Issuer,

                ValidateAudience = true,
                ValidAudience = _opt.Audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_opt.SigningKey))
            };

            try
            {
                var principal = handler.ValidateToken(token, parameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
