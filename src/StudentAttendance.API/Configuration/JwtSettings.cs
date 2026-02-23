
namespace StudentAttendance.API.Configuration;

public class JwtSettings
{
    public string Key {get;set;} = default;
    public string Issuer {get;set;} = default;
    public string Audience {get;set;} = default;
    public int AccessTokenExpiration {get;set;} = default;

    public int RefreashTokenDays {get;set;} = default;

}