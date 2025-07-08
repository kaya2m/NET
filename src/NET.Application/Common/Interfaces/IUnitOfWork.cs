using NET.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace NET.Application.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Tenant> Tenants { get; }
        IRepository<Building> Buildings { get; }
        IRepository<BuildingUnit> BuildingUnits { get; }
        IRepository<Resident> Residents { get; }
        IRepository<ResidentUnit> ResidentUnits { get; }
        IRepository<Payment> Payments { get; }
        IRepository<Invoice> Invoices { get; }
        IRepository<Expense> Expenses { get; }
        IRepository<MaintenanceRequest> MaintenanceRequests { get; }
        IRepository<User> Users { get; }
        IRepository<Role> Roles { get; }
        IRepository<UserRole> UserRoles { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}