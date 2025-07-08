using System;

namespace NET.Domain.Exceptions
{
    public class TenantException : DomainException
    {
        public string TenantId { get; }

        public TenantException(string tenantId) : base($"Tenant operation failed for tenant: {tenantId}")
        {
            TenantId = tenantId;
        }

        public TenantException(string tenantId, string message) : base(message)
        {
            TenantId = tenantId;
        }

        public TenantException(string tenantId, string message, Exception innerException) : base(message, innerException)
        {
            TenantId = tenantId;
        }
    }
}