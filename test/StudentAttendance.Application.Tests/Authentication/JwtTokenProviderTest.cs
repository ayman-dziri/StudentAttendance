using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentAttendance.src.StudentAttendance.Domain.Auth;
using StudentAttendance.src.StudentAttendance.Infrastructure.Auth;
using StudentAttendance.src.StudentAttendance.Infrastructure.Providers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentAttendance.test.StudentAttendance.Application.Tests.Authentication
{
    [TestClass]
    public class JwtTokenProviderTest
    {

        //private const string Issuer = "test-issuer";
        //private const string Audience = "test-audience";


        //private const string SigningKey = "THIS_IS_A_VERY_LONG_TEST_SIGNING_KEY_32+_CHARS";

        //private static JwtTokenProvider CreateSut(int expMinutes = 60)
        //{
        //    var options = Options.Create(new JwtOptions
        //    {
        //        Issuer = Issuer,
        //        Audience = Audience,
        //        SigningKey = SigningKey,
        //        ExpMinuts = expMinutes
        //    });

        //    return new JwtTokenProvider(options);
        //}

        //private static JwtUserDescriptor CreateUser(
        //    string userId = "123",
        //    string email = "ayman@gmail.com",
        //    IReadOnlyDictionary<string, string>? claims = null)
        //{
        //    claims ??= new Dictionary<string, string>();
        //    return new JwtUserDescriptor(userId, email, claims);
        //}

        //private static JwtSecurityToken ReadJwt(string token)
        //{
        //    var handler = new JwtSecurityTokenHandler();
        //    var jwt = handler.ReadToken(token) as JwtSecurityToken;
        //    Assert.IsNotNull(jwt, "Le token généré n'est pas un JwtSecurityToken lisible.");
        //    return jwt!;
        //}

        //private static ClaimsPrincipal ValidateAndGetPrincipal(string token)
        //{
        //    var handler = new JwtSecurityTokenHandler();

        //    var parameters = new TokenValidationParameters
        //    {
        //        ValidateIssuer = true,
        //        ValidIssuer = Issuer,

        //        ValidateAudience = true,
        //        ValidAudience = Audience,

        //        ValidateLifetime = true,
        //        ClockSkew = TimeSpan.FromSeconds(30),

        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey))
        //    };

        //    return handler.ValidateToken(token, parameters, out _);
        //}

        //private static string? GetClaimValue(JwtSecurityToken jwt, string claimType)
        //{
        //    return jwt.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        //}

        //[TestMethod]
        //public void GenerateToken_ShouldReturn_NotEmpty_JwtString()
        //{
        //    var sut = CreateSut();
        //    var user = CreateUser();

        //    var token = sut.GenerateToken(user);

        //    Assert.IsFalse(string.IsNullOrWhiteSpace(token), "Le token ne doit pas être null/vide.");

        //    var jwt = ReadJwt(token);
        //    Assert.IsTrue(jwt.RawData.Length > 0, "Le JWT doit contenir RawData.");
        //}
    }
}
