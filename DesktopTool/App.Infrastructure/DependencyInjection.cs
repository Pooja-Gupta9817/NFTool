using DesktopTool.App.Core;
using DesktopTool.App.Core.Interfaces;
using DesktopTool.App.Core.Models;
using DesktopTool.App.Data;
using DesktopTool.App.Infrastructure.Repository;
using DesktopTool.App.Service;
using DesktopTool.App.UI.View;
using DesktopTool.App.UI.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
            services.AddTransient<StudentViewModel>();
            services.AddTransient<TeacherViewModel>();

            services.AddTransient<UploadPdfViewModel>();
            services.AddTransient<UploadMarksViewModel>();
            services.AddTransient<StudentListViewModel>();

            // Register Views
            services.AddTransient<LoginView>();

            //  Register AuthService
            // services.AddHttpClient<IAuthService, AuthService>();

            services.AddSingleton<IAuthService>(provider =>
            {
                var clientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var client = clientFactory.CreateClient();
                client.BaseAddress = new Uri("http://localhost:7071");

                var dbContext = provider.GetRequiredService<ApplicationDbContext>();
                return new AuthService(dbContext, client);
            });

            services.AddHttpClient();


            services.AddSingleton<TeacherView>();
            services.AddSingleton<StudentView>();
            services.AddSingleton<MainWindow>();

            services.AddSingleton<UploadPdfView>();
            services.AddSingleton<UploadMarksView>();
            services.AddSingleton<StudentListView>();

            services.AddSingleton<IRoleBasedDashboardService, RoleBasedDashboardService>();
            services.AddSingleton<IUploadedFileRepository, UploadedFileRepository>();

            return services;


        }
    }

}
