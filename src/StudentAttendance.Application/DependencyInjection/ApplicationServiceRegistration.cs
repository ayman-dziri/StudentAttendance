using Microsoft.Extensions.DependencyInjection;

namespace StudentAttendance.src.StudentAttendance.Application.DependencyInjection
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<Interfaces.IGroupService, Services.GroupService>();
            return services;
        }
    }
}


