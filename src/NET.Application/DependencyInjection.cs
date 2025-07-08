using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NET.Application.Services;
using System.Reflection;

namespace NET.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Application Services
            services.AddScoped<IBuildingService, BuildingService>();
            services.AddScoped<IResidentService, ResidentService>();
            services.AddScoped<IFinancialService, FinancialService>();
            //services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<IUserService, UserService>();
            //services.AddScoped<ITenantManagementService, TenantManagementService>();

            // Additional Services
            //services.AddScoped<IReportService, ReportService>();
            //services.AddScoped<INotificationService, NotificationService>();
            //services.AddScoped<IDashboardService, DashboardService>();

            return services;
        }
    }
}