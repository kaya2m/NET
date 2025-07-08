using Microsoft.EntityFrameworkCore;
using NET.Domain.Entities;
using NET.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NET.Infrastructure.Data.Repositories
{
    public class ResidentRepository : BaseRepository<Resident>
    {
        public ResidentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Resident>> GetActiveResidentsAsync()
        {
            return await _dbSet
                .Where(r => r.IsActive && !r.IsDeleted)
                .OrderBy(r => r.FirstName)
                .ThenBy(r => r.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Resident>> GetResidentsByBuildingAsync(Guid buildingId)
        {
            return await _dbSet
                .Include(r => r.ResidentUnits)
                    .ThenInclude(ru => ru.BuildingUnit)
                .Where(r => !r.IsDeleted &&
                           r.ResidentUnits.Any(ru => ru.BuildingUnit.BuildingId == buildingId && ru.IsActive))
                .ToListAsync();
        }

        public async Task<IEnumerable<Resident>> GetResidentsByUnitAsync(Guid unitId)
        {
            return await _dbSet
                .Include(r => r.ResidentUnits)
                .Where(r => !r.IsDeleted &&
                           r.ResidentUnits.Any(ru => ru.BuildingUnitId == unitId && ru.IsActive))
                .ToListAsync();
        }

        public async Task<Resident?> GetResidentWithUnitsAsync(Guid id)
        {
            return await _dbSet
                .Include(r => r.ResidentUnits)
                    .ThenInclude(ru => ru.BuildingUnit)
                        .ThenInclude(u => u.Building)
                .Include(r => r.Payments)
                .Include(r => r.MaintenanceRequests)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<IEnumerable<Resident>> SearchResidentsAsync(string searchTerm)
        {
            return await _dbSet
                .Where(r => !r.IsDeleted &&
                           (r.FirstName.Contains(searchTerm) ||
                            r.LastName.Contains(searchTerm) ||
                            (r.Email != null && r.Email.Contains(searchTerm)) ||
                            (r.Phone != null && r.Phone.Contains(searchTerm))))
                .OrderBy(r => r.FirstName)
                .ThenBy(r => r.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Resident>> GetResidentsWithoutUnitsAsync()
        {
            return await _dbSet
                .Include(r => r.ResidentUnits)
                .Where(r => !r.IsDeleted &&
                           r.IsActive &&
                           !r.ResidentUnits.Any(ru => ru.IsActive))
                .OrderBy(r => r.FirstName)
                .ThenBy(r => r.LastName)
                .ToListAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null)
        {
            var query = _dbSet.Where(r => !r.IsDeleted && r.Email == email);

            if (excludeId.HasValue)
            {
                query = query.Where(r => r.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }
    }
}