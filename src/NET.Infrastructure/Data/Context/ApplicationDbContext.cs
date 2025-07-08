using Microsoft.EntityFrameworkCore;
using NET.Application.Common.Interfaces;
using NET.Domain.Entities;
using NET.Domain.Entities.Common;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NET.Infrastructure.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ITenantContextProvider _tenantContextProvider;
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ITenantContextProvider tenantContextProvider,
            ICurrentUserService currentUserService) : base(options)
        {
            _tenantContextProvider = tenantContextProvider;
            _currentUserService = currentUserService;
        }

        // DbSets
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<BuildingUnit> BuildingUnits { get; set; }
        public DbSet<Resident> Residents { get; set; }
        public DbSet<ResidentUnit> ResidentUnits { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            ApplyGlobalQueryFilters(modelBuilder);

            ConfigureTableNames(modelBuilder);

            ConfigureRelationships(modelBuilder);

            ConfigureIndexes(modelBuilder);
        }

        private void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Building>()
                .HasQueryFilter(b => b.TenantId == _tenantContextProvider.GetCurrentTenantId());

            modelBuilder.Entity<BuildingUnit>()
                .HasQueryFilter(u => u.TenantId == _tenantContextProvider.GetCurrentTenantId());

            modelBuilder.Entity<Resident>()
                .HasQueryFilter(r => r.TenantId == _tenantContextProvider.GetCurrentTenantId());

            modelBuilder.Entity<ResidentUnit>()
                .HasQueryFilter(ru => ru.TenantId == _tenantContextProvider.GetCurrentTenantId());

            modelBuilder.Entity<Payment>()
                .HasQueryFilter(p => p.TenantId == _tenantContextProvider.GetCurrentTenantId());

            modelBuilder.Entity<Invoice>()
                .HasQueryFilter(i => i.TenantId == _tenantContextProvider.GetCurrentTenantId());

            modelBuilder.Entity<Expense>()
                .HasQueryFilter(e => e.TenantId == _tenantContextProvider.GetCurrentTenantId());

            modelBuilder.Entity<MaintenanceRequest>()
                .HasQueryFilter(m => m.TenantId == _tenantContextProvider.GetCurrentTenantId());

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => u.TenantId == _tenantContextProvider.GetCurrentTenantId());

            modelBuilder.Entity<Role>()
                .HasQueryFilter(r => r.TenantId == _tenantContextProvider.GetCurrentTenantId());

            modelBuilder.Entity<UserRole>()
                .HasQueryFilter(ur => ur.TenantId == _tenantContextProvider.GetCurrentTenantId());

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var condition = Expression.Equal(property, Expression.Constant(false));
                    var lambda = Expression.Lambda(condition, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }

        private void ConfigureTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>().ToTable("tenants");
            modelBuilder.Entity<Building>().ToTable("buildings");
            modelBuilder.Entity<BuildingUnit>().ToTable("building_units");
            modelBuilder.Entity<Resident>().ToTable("residents");
            modelBuilder.Entity<ResidentUnit>().ToTable("resident_units");
            modelBuilder.Entity<Payment>().ToTable("payments");
            modelBuilder.Entity<Invoice>().ToTable("invoices");
            modelBuilder.Entity<Expense>().ToTable("expenses");
            modelBuilder.Entity<MaintenanceRequest>().ToTable("maintenance_requests");
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Role>().ToTable("roles");
            modelBuilder.Entity<UserRole>().ToTable("user_roles");
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Building>()
                .HasOne(b => b.Tenant)
                .WithMany(t => t.Buildings)
                .HasForeignKey(b => b.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BuildingUnit>()
                .HasOne(u => u.Building)
                .WithMany(b => b.BuildingUnits)
                .HasForeignKey(u => u.BuildingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Resident relationships
            modelBuilder.Entity<ResidentUnit>()
                .HasOne(ru => ru.Resident)
                .WithMany(r => r.ResidentUnits)
                .HasForeignKey(ru => ru.ResidentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ResidentUnit>()
                .HasOne(ru => ru.BuildingUnit)
                .WithMany(u => u.ResidentUnits)
                .HasForeignKey(ru => ru.BuildingUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment relationships
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Resident)
                .WithMany(r => r.Payments)
                .HasForeignKey(p => p.ResidentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.BuildingUnit)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.BuildingUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Invoice relationships
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.BuildingUnit)
                .WithMany(u => u.Invoices)
                .HasForeignKey(i => i.BuildingUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Resident)
                .WithMany()
                .HasForeignKey(i => i.ResidentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Expense relationships
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Building)
                .WithMany(b => b.Expenses)
                .HasForeignKey(e => e.BuildingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.ApprovedByUser)
                .WithMany(u => u.ApprovedExpenses)
                .HasForeignKey(e => e.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Maintenance relationships
            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Building)
                .WithMany(b => b.MaintenanceRequests)
                .HasForeignKey(m => m.BuildingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(m => m.BuildingUnit)
                .WithMany(u => u.MaintenanceRequests)
                .HasForeignKey(m => m.BuildingUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Resident)
                .WithMany(r => r.MaintenanceRequests)
                .HasForeignKey(m => m.ResidentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(m => m.AssignedToUser)
                .WithMany(u => u.AssignedMaintenanceRequests)
                .HasForeignKey(m => m.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(m => m.ApprovedByUser)
                .WithMany(u => u.ApprovedMaintenanceRequests)
                .HasForeignKey(m => m.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // User relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.Tenant)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Building)
                .WithMany(b => b.Users)
                .HasForeignKey(u => u.BuildingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Resident)
                .WithMany()
                .HasForeignKey(u => u.ResidentId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserRole relationships
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>()
                .HasIndex(t => t.Subdomain)
                .IsUnique();

            modelBuilder.Entity<Building>()
                .HasIndex(b => new { b.TenantId, b.Name });

            modelBuilder.Entity<BuildingUnit>()
                .HasIndex(u => new { u.BuildingId, u.UnitNumber })
                .IsUnique();

            modelBuilder.Entity<Resident>()
                .HasIndex(r => r.Email);

            modelBuilder.Entity<Resident>()
                .HasIndex(r => new { r.TenantId, r.Email });

            // Payment indexes
            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.ReferenceNumber);

            modelBuilder.Entity<Payment>()
                .HasIndex(p => new { p.TenantId, p.PaymentDate });

            modelBuilder.Entity<Payment>()
                .HasIndex(p => new { p.ResidentId, p.PaymentDate });

            // Invoice indexes
            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique();

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => new { i.TenantId, i.InvoiceDate });

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => new { i.BuildingUnitId, i.InvoiceDate });

            // Expense indexes
            modelBuilder.Entity<Expense>()
                .HasIndex(e => new { e.TenantId, e.ExpenseDate });

            modelBuilder.Entity<Expense>()
                .HasIndex(e => new { e.BuildingId, e.ExpenseDate });

            // Maintenance indexes
            modelBuilder.Entity<MaintenanceRequest>()
                .HasIndex(m => new { m.TenantId, m.Status });

            modelBuilder.Entity<MaintenanceRequest>()
                .HasIndex(m => new { m.BuildingId, m.Status });

            modelBuilder.Entity<MaintenanceRequest>()
                .HasIndex(m => new { m.AssignedToUserId, m.Status });

            // User indexes
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => new { u.TenantId, u.Role });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();

            UpdateTenantFields();

            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            // Update audit fields
            UpdateAuditFields();

            // Update tenant fields
            UpdateTenantFields();

            return base.SaveChanges();
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            var currentUser = _currentUserService.UserName ?? "System";
            var currentTime = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = currentTime;
                    entity.CreatedBy = currentUser;
                }

                entity.UpdatedAt = currentTime;
                entity.UpdatedBy = currentUser;
            }
        }

        private void UpdateTenantFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is ITenantEntity && e.State == EntityState.Added);

            var currentTenantId = _tenantContextProvider.GetCurrentTenantId();

            foreach (var entry in entries)
            {
                var entity = (ITenantEntity)entry.Entity;
                if (entity.TenantId == Guid.Empty)
                {
                    entity.TenantId = currentTenantId;
                }
            }
        }
    }
}