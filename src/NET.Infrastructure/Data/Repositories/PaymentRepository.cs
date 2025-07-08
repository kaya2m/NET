using Microsoft.EntityFrameworkCore;
using NET.Domain.Entities;
using NET.Domain.Enums;
using NET.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NET.Infrastructure.Data.Repositories
{
    public class PaymentRepository : BaseRepository<Payment>
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByResidentAsync(Guid residentId)
        {
            return await _dbSet
                .Include(p => p.Resident)
                .Include(p => p.BuildingUnit)
                    .ThenInclude(u => u.Building)
                .Include(p => p.Invoice)
                .Where(p => p.ResidentId == residentId && !p.IsDeleted)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByBuildingAsync(Guid buildingId)
        {
            return await _dbSet
                .Include(p => p.Resident)
                .Include(p => p.BuildingUnit)
                    .ThenInclude(u => u.Building)
                .Where(p => !p.IsDeleted && p.BuildingUnit.BuildingId == buildingId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            return await _dbSet
                .Include(p => p.Resident)
                .Include(p => p.BuildingUnit)
                    .ThenInclude(u => u.Building)
                .Where(p => p.Status == status && !p.IsDeleted)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _dbSet
                .Include(p => p.Resident)
                .Include(p => p.BuildingUnit)
                    .ThenInclude(u => u.Building)
                .Where(p => !p.IsDeleted &&
                           p.PaymentDate >= fromDate &&
                           p.PaymentDate <= toDate)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetOverduePaymentsAsync()
        {
            return await _dbSet
                .Include(p => p.Resident)
                .Include(p => p.BuildingUnit)
                    .ThenInclude(u => u.Building)
                .Where(p => !p.IsDeleted && p.Status == PaymentStatus.Overdue)
                .OrderBy(p => p.DueDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalPaymentsByStatusAsync(PaymentStatus status)
        {
            return await _dbSet
                .Where(p => p.Status == status && !p.IsDeleted)
                .SumAsync(p => p.Amount);
        }

        public async Task<decimal> GetTotalPaymentsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _dbSet
                .Where(p => !p.IsDeleted &&
                           p.PaymentDate >= fromDate &&
                           p.PaymentDate <= toDate &&
                           p.Status == PaymentStatus.Paid)
                .SumAsync(p => p.Amount);
        }

        public async Task<Payment?> GetPaymentByReferenceAsync(string referenceNumber)
        {
            return await _dbSet
                .Include(p => p.Resident)
                .Include(p => p.BuildingUnit)
                .Include(p => p.Invoice)
                .FirstOrDefaultAsync(p => p.ReferenceNumber == referenceNumber && !p.IsDeleted);
        }
    }
}