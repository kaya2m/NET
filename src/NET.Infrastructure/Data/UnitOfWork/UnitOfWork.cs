using Microsoft.EntityFrameworkCore.Storage;
using NET.Application.Common.Interfaces;
using NET.Domain.Entities;
using NET.Infrastructure.Data.Context;
using NET.Infrastructure.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace NET.Infrastructure.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        private IRepository<Tenant>? _tenants;
        private IRepository<Building>? _buildings;
        private IRepository<BuildingUnit>? _buildingUnits;
        private IRepository<Resident>? _residents;
        private IRepository<ResidentUnit>? _residentUnits;
        private IRepository<Payment>? _payments;
        private IRepository<Invoice>? _invoices;
        private IRepository<Expense>? _expenses;
        private IRepository<MaintenanceRequest>? _maintenanceRequests;
        private IRepository<User>? _users;
        private IRepository<Role>? _roles;
        private IRepository<UserRole>? _userRoles;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<Tenant> Tenants =>
            _tenants ??= new BaseRepository<Tenant>(_context);

        public IRepository<Building> Buildings =>
            _buildings ??= new BuildingRepository(_context);

        public IRepository<BuildingUnit> BuildingUnits =>
            _buildingUnits ??= new BaseRepository<BuildingUnit>(_context);

        public IRepository<Resident> Residents =>
            _residents ??= new ResidentRepository(_context);

        public IRepository<ResidentUnit> ResidentUnits =>
            _residentUnits ??= new BaseRepository<ResidentUnit>(_context);

        public IRepository<Payment> Payments =>
            _payments ??= new PaymentRepository(_context);

        public IRepository<Invoice> Invoices =>
            _invoices ??= new BaseRepository<Invoice>(_context);

        public IRepository<Expense> Expenses =>
            _expenses ??= new BaseRepository<Expense>(_context);

        public IRepository<MaintenanceRequest> MaintenanceRequests =>
            _maintenanceRequests ??= new MaintenanceRepository(_context);

        public IRepository<User> Users =>
            _users ??= new BaseRepository<User>(_context);

        public IRepository<Role> Roles =>
            _roles ??= new BaseRepository<Role>(_context);

        public IRepository<UserRole> UserRoles =>
            _userRoles ??= new BaseRepository<UserRole>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}