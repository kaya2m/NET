namespace NET.Domain.Entities.Common
{
    public interface ITenantEntity
    {
        Guid TenantId { get; set; }
    }
}