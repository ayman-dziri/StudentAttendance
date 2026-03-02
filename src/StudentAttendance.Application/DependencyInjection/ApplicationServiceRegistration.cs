using Microsoft.Extensions.DependencyInjection;
using StudentAttendance.src.StudentAttendance.Application.Interfaces;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Application.Services;

namespace StudentAttendance.src.StudentAttendance.Application.DependencyInjection
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<Interfaces.IGroupService, Services.GroupService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISessionsService, SessionsService>();
            services.AddScoped<ISessionConflictValidator, SessionConflictValidator>();
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}


