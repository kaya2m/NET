using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NET.Application.Common.Interfaces;
using NET.Domain.Entities;
using NET.Infrastructure.Data.Context;
using System;
using System.Threading.Tasks;

namespace NET.Infrastructure.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantContextProvider _tenantContextProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Func<ApplicationDbContext> _contextFactory;

        public TenantService(
            ITenantContextProvider tenantContextProvider, 
            IHttpContextAccessor httpContextAccessor,
            Func<ApplicationDbContext> contextFactory)
        {
            _tenantContextProvider = tenantContextProvider;
            _httpContextAccessor = httpContextAccessor;
            _contextFactory = contextFactory;
        }

        public Guid GetCurrentTenantId()
        {
            // First check if we can resolve via subdomain with database lookup
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var host = httpContext.Request.Host.Host;
                var subdomain = ExtractSubdomain(host);

                if (!string.IsNullOrEmpty(subdomain))
                {
                    using var context = _contextFactory();
                    var tenant = context.Tenants
                        .FirstOrDefault(t => t.Subdomain == subdomain && !t.IsDeleted);

                    if (tenant != null)
                    {
                        _tenantContextProvider.SetCurrentTenantId(tenant.Id);
                        return tenant.Id;
                    }
                }
            }

            // Fall back to context provider for other methods
            return _tenantContextProvider.GetCurrentTenantId();
        }

        public async Task<Tenant?> GetCurrentTenantAsync()
        {
            var tenantId = GetCurrentTenantId();
            using var context = _contextFactory();
            return await context.Tenants
                .FirstOrDefaultAsync(t => t.Id == tenantId && !t.IsDeleted);
        }

        public void SetTenant(Guid tenantId)
        {
            _tenantContextProvider.SetCurrentTenantId(tenantId);
        }

        public void SetTenant(string subdomain)
        {
            using var context = _contextFactory();
            var tenant = context.Tenants
                .FirstOrDefault(t => t.Subdomain == subdomain && !t.IsDeleted);

            if (tenant != null)
            {
                _tenantContextProvider.SetCurrentTenantId(tenant.Id);
            }
            else
            {
                throw new ArgumentException($"Tenant with subdomain '{subdomain}' not found");
            }
        }

        public async Task<bool> ValidateTenantAsync(Guid tenantId)
        {
            using var context = _contextFactory();
            return await context.Tenants
                .AnyAsync(t => t.Id == tenantId && t.IsActive && !t.IsDeleted);
        }

        public async Task<Tenant?> GetTenantBySubdomainAsync(string subdomain)
        {
            using var context = _contextFactory();
            return await context.Tenants
                .FirstOrDefaultAsync(t => t.Subdomain == subdomain && !t.IsDeleted);
        }

        private string? ExtractSubdomain(string host)
        {
            // Extract subdomain from host
            // Example: tenant1.yourdomain.com -> tenant1

            if (string.IsNullOrEmpty(host))
                return null;

            // Remove port if present
            var hostWithoutPort = host.Split(':')[0];

            // Split by dots
            var parts = hostWithoutPort.Split('.');

            // If we have at least 3 parts (subdomain.domain.tld), return the first part
            if (parts.Length >= 3)
            {
                return parts[0];
            }

            return null;
        }
    }
}