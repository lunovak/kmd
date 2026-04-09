using Kmd.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Kmd.Web.Services;

public class PerformanceService
{
    private readonly ApplicationDbContext _db;

    public PerformanceService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<Performance>> GetAllAsync(int? seasonId = null, int? waveId = null, int? theatreId = null)
    {
        var query = _db.Performances
            .Include(p => p.Play)
            .Include(p => p.Theatre)
            .Include(p => p.ReservationWave).ThenInclude(w => w.Season)
            .Include(p => p.Reservations)
            .AsQueryable();

        if (seasonId.HasValue)
            query = query.Where(p => p.ReservationWave.SeasonId == seasonId.Value);
        if (waveId.HasValue)
            query = query.Where(p => p.ReservationWaveId == waveId.Value);
        if (theatreId.HasValue)
            query = query.Where(p => p.TheatreId == theatreId.Value);

        return await query.OrderBy(p => p.DateTime).ToListAsync();
    }

    public async Task<List<Performance>> GetOpenPerformancesAsync()
    {
        var now = DateTime.UtcNow;
        return await _db.Performances
            .Include(p => p.Play)
            .Include(p => p.Theatre)
            .Include(p => p.ReservationWave).ThenInclude(w => w.Season)
            .Include(p => p.Reservations)
            .Where(p => p.DateTime > now && p.ReservationWave.StartDate <= now)
            .OrderBy(p => p.ReservationWave.StartDate)
            .ThenBy(p => p.DateTime)
            .ToListAsync();
    }

    public async Task<Performance?> GetByIdAsync(int id) =>
        await _db.Performances
            .Include(p => p.Play).ThenInclude(pl => pl.Theatre)
            .Include(p => p.Theatre)
            .Include(p => p.ReservationWave).ThenInclude(w => w.Season)
            .Include(p => p.Reservations)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task CreateAsync(Performance performance)
    {
        _db.Performances.Add(performance);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Performance performance)
    {
        _db.Performances.Update(performance);
        await _db.SaveChangesAsync();
    }

    public async Task<int> GetActiveReservationCountAsync(int performanceId) =>
        await _db.Reservations.CountAsync(r => r.PerformanceId == performanceId && r.Status == ReservationStatus.Active);

    public async Task<bool> CanDeleteAsync(int id) =>
        !await _db.Reservations.AnyAsync(r => r.PerformanceId == id && r.Status == ReservationStatus.Active);

    public async Task DeleteAsync(int id)
    {
        var perf = await _db.Performances.FindAsync(id);
        if (perf != null)
        {
            _db.Performances.Remove(perf);
            await _db.SaveChangesAsync();
        }
    }
}
