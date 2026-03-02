namespace StudentAttendance.src.StudentAttendance.Infrastructure.Auth
{
    public static class JwtConfiguration
    {

        public static IServiceCollection AddJwtOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            return services;
        }
    }
}
