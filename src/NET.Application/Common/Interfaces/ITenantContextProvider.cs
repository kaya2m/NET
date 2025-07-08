using System;

namespace NET.Application.Common.Interfaces
{
    public interface ITenantContextProvider
    {
        Guid GetCurrentTenantId();
        void SetCurrentTenantId(Guid tenantId);
        bool HasTenantContext();
    }
}