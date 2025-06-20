using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DesktopTool.App.Core;
using DesktopTool.App.Core.Models;
using DesktopTool.App.Infrastructure.Repository;

namespace DesktopTool.App.Infrastructure.Service
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddDbContext<UploadDbContext>(options =>
                 options.UseSqlServer(connectionString));

            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
