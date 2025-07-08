using Microsoft.AspNetCore.Http;
using NET.Application.Common.Interfaces;
using System;

namespace NET.Infrastructure.Services
{
    public class TenantContextProvider : ITenantContextProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Guid? _currentTenantId;

        public TenantContextProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetCurrentTenantId()
        {
            if (_currentTenantId.HasValue)
                return _currentTenantId.Value;

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                // Try from header
                if (httpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader))
                {
                    if (Guid.TryParse(tenantHeader, out var tenantId))
                    {
                        _currentTenantId = tenantId;
                        return _currentTenantId.Value;
                    }
                }

                // Try from JWT claims
                var tenantClaim = httpContext.User.FindFirst("tenant_id");
                if (tenantClaim != null && Guid.TryParse(tenantClaim.Value, out var claimTenantId))
                {
                    _currentTenantId = claimTenantId;
                    return _currentTenantId.Value;
                }

                // Try from route or query parameters
                if (httpContext.Request.Query.TryGetValue("tenantId", out var queryTenantId))
                {
                    if (Guid.TryParse(queryTenantId, out var queryParsedTenantId))
                    {
                        _currentTenantId = queryParsedTenantId;
                        return _currentTenantId.Value;
                    }
                }
            }

            throw new InvalidOperationException("No tenant context available");
        }

        public void SetCurrentTenantId(Guid tenantId)
        {
            _currentTenantId = tenantId;
        }

        public bool HasTenantContext()
        {
            try
            {
                GetCurrentTenantId();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}