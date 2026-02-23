using Microsoft.Extensions.Options;

using Microsoft.IdentityModel.Tokens;
using StudentAttendance.API.Configuration;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentAttendance.src.StudentAttendance.Application.Services;

    public class JwtTokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        
        public JwtTokenService(IOptions<JwtSettings> jwtOptions) //injection automatique si tu ajoute la ocnfiguration dans le program.cs
    {
            _jwtSettings = jwtOptions.Value;
        }

        public (string AccessToken, DateTime ExpiresAtUtc) CreateAccessToken(string userId, string email, string role)
    {
        var now = DateTime.UtcNow; //time actuel avant expiration
        var exp = now.AddMinutes(_jwtSettings.AccessTokenExpiration); //le temps de l'expiration du token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)); // clé de signature du token symmetrique que tu vas utiliser pour signer le token
        var creds = new SigningCredentials(key , SecurityAlgorithms.HmacSha256); //algorithme de signature du token

        var claimss = new List<Claim> //jwt contient  claims qui sont des informations sur l'utilisateur que tu veux inclure dans le token, comme son id, son email et son role
        {
            new(JwtRegisteredClaimNames.Sub, userId), // (cle , valeur)
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer, //qui a demandé le token 
            audience: _jwtSettings.Audience, //qui est censé d'utiliser
            claims: claimss, //identité de l'utilisateur + son role
            notBefore: now, //token invalide avant ce temps 
            expires: exp, //DateOnly dexpiration
            signingCredentials: creds //comment le signer
            );

        return (new JwtSecurityTokenHandler().WriteToken(token), exp); //transforme l'jbjet jwt en string format (xxxxxx.yyyy.zzzzzz) et
                                                                       //retourne le token + date d'expiration
    }

}

