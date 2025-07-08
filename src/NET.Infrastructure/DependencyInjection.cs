using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NET.Application.Common.Interfaces;
using NET.Infrastructure.Authentication;
using NET.Infrastructure.Data.Context;
using NET.Infrastructure.Data.UnitOfWork;
using NET.Infrastructure.Services;

namespace NET.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Authentication Services
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            // Infrastructure Services
            services.AddScoped<ITenantContextProvider, TenantContextProvider>();
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IEmailService, EmailService>();
            
            // Context factory for breaking circular dependencies
            services.AddScoped<Func<ApplicationDbContext>>(provider => () => provider.GetRequiredService<ApplicationDbContext>());

            // HTTP Context Accessor
            services.AddHttpContextAccessor();

            return services;
        }
    }
}