using DesktopTool.App.Core;
using DesktopTool.App.Core.Models;
using DesktopTool.App.Data;
using DesktopTool.App.Service;
using DesktopTool.App.UI.View;
using DesktopTool.App.UI.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopTool.App.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            connectionString = "Server=localhost;Database=StudentToolDb;Trusted_Connection=True;TrustServerCertificate=True;";
           
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString)); // This connects to Azure SQL

            services.AddScoped<IUserRepository, UserRepository>();

            // Register ViewModels
            services.AddTransient<LoginViewModel>();

            // Register Views
            services.AddTransient<LoginView>();

            // ✅ Register AuthService
            services.AddScoped<IAuthService, AuthService>();

            return services;


        }
    }

}
