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
    public class MaintenanceRepository : BaseRepository<MaintenanceRequest>
    {
        public MaintenanceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetRequestsByBuildingAsync(Guid buildingId)
        {
            return await _dbSet
                .Include(m => m.Building)
                .Include(m => m.BuildingUnit)
                .Include(m => m.Resident)
                .Include(m => m.AssignedToUser)
                .Where(m => m.BuildingId == buildingId && !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetRequestsByStatusAsync(MaintenanceStatus status)
        {
            return await _dbSet
                .Include(m => m.Building)
                .Include(m => m.BuildingUnit)
                .Include(m => m.Resident)
                .Include(m => m.AssignedToUser)
                .Where(m => m.Status == status && !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetRequestsByPriorityAsync(Priority priority)
        {
            return await _dbSet
                .Include(m => m.Building)
                .Include(m => m.BuildingUnit)
                .Include(m => m.Resident)
                .Include(m => m.AssignedToUser)
                .Where(m => m.Priority == priority && !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetAssignedRequestsAsync(Guid userId)
        {
            return await _dbSet
                .Include(m => m.Building)
                .Include(m => m.BuildingUnit)
                .Include(m => m.Resident)
                .Where(m => m.AssignedToUserId == userId && !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetOverdueRequestsAsync()
        {
            return await _dbSet
                .Include(m => m.Building)
                .Include(m => m.BuildingUnit)
                .Include(m => m.Resident)
                .Include(m => m.AssignedToUser)
                .Where(m => !m.IsDeleted &&
                           m.ScheduledDate.HasValue &&
                           m.ScheduledDate < DateTime.UtcNow &&
                           m.Status != MaintenanceStatus.Completed &&
                           m.Status != MaintenanceStatus.Cancelled)
                .OrderBy(m => m.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetHighPriorityRequestsAsync()
        {
            return await _dbSet
                .Include(m => m.Building)
                .Include(m => m.BuildingUnit)
                .Include(m => m.Resident)
                .Include(m => m.AssignedToUser)
                .Where(m => !m.IsDeleted &&
                           m.Priority >= Priority.High &&
                           m.Status != MaintenanceStatus.Completed &&
                           m.Status != MaintenanceStatus.Cancelled)
                .OrderByDescending(m => m.Priority)
                .ThenByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetPendingApprovalsAsync()
        {
            return await _dbSet
                .Include(m => m.Building)
                .Include(m => m.BuildingUnit)
                .Include(m => m.Resident)
                .Where(m => !m.IsDeleted && m.RequiresApproval && !m.IsApproved)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalMaintenanceCostAsync(DateTime fromDate, DateTime toDate)
        {
            return await _dbSet
                .Where(m => !m.IsDeleted &&
                           m.CompletedDate.HasValue &&
                           m.CompletedDate >= fromDate &&
                           m.CompletedDate <= toDate &&
                           m.ActualCost.HasValue)
                .SumAsync(m => m.ActualCost.Value);
        }

        public async Task<int> GetRequestCountByStatusAsync(MaintenanceStatus status)
        {
            return await _dbSet
                .CountAsync(m => m.Status == status && !m.IsDeleted);
        }

        public async Task<double> GetAverageCompletionTimeAsync()
        {
            var completedRequests = await _dbSet
                .Where(m => !m.IsDeleted &&
                           m.Status == MaintenanceStatus.Completed &&
                           m.CompletedDate.HasValue)
                .Select(m => new { m.CreatedAt, m.CompletedDate })
                .ToListAsync();

            if (!completedRequests.Any())
                return 0;

            return completedRequests.Average(r => (r.CompletedDate.Value - r.CreatedAt).TotalHours);
        }
    }
}