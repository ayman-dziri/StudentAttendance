using Microsoft.Extensions.DependencyInjection;
using StudentAttendance.src.StudentAttendance.Application.Interfaces;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Application.Services;
using StudentAttendanceV2.src.StudentAttendance.Application.Interfaces;
using StudentAttendanceV2.src.StudentAttendance.Application.Services;

namespace StudentAttendance.src.StudentAttendance.Application.DependencyInjection
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISessionsService, SessionsService>();
            services.AddScoped<ISessionConflictValidator, SessionConflictValidator>();
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService , CurrentUserService>();
            services.AddScoped<IAuthService , AuthService>();

            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            return services;
        }
    }
}


