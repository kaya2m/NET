using Microsoft.EntityFrameworkCore;
using NET.Domain.Entities;
using NET.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NET.Infrastructure.Data.Repositories
{
    public class BuildingRepository : BaseRepository<Building>
    {
        public BuildingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Building>> GetActiveBuildingsAsync()
        {
            return await _dbSet
                .Where(b => b.IsActive && !b.IsDeleted)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Building>> GetBuildingsWithUnitsAsync()
        {
            return await _dbSet
                .Include(b => b.BuildingUnits)
                .Where(b => !b.IsDeleted)
                .ToListAsync();
        }

        public async Task<Building?> GetBuildingWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(b => b.BuildingUnits)
                .Include(b => b.MaintenanceRequests)
                .Include(b => b.Expenses)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        }

        public async Task<IEnumerable<Building>> SearchBuildingsAsync(string searchTerm)
        {
            return await _dbSet
                .Where(b => !b.IsDeleted &&
                           (b.Name.Contains(searchTerm) ||
                            b.Address.Contains(searchTerm)))
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<int> GetTotalUnitsCountAsync(Guid buildingId)
        {
            var building = await _dbSet
                .Include(b => b.BuildingUnits)
                .FirstOrDefaultAsync(b => b.Id == buildingId && !b.IsDeleted);

            return building?.BuildingUnits.Count(u => !u.IsDeleted) ?? 0;
        }

        public async Task<int> GetOccupiedUnitsCountAsync(Guid buildingId)
        {
            var building = await _dbSet
                .Include(b => b.BuildingUnits)
                .FirstOrDefaultAsync(b => b.Id == buildingId && !b.IsDeleted);

            return building?.BuildingUnits.Count(u => !u.IsDeleted && u.IsOccupied) ?? 0;
        }
    }
}