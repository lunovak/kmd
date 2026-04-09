using Kmd.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Kmd.Web.Services;

public class ReservationWaveService
{
    private readonly ApplicationDbContext _db;

    public ReservationWaveService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<ReservationWave>> GetBySeasonAsync(int seasonId) =>
        await _db.ReservationWaves
            .Include(w => w.Season)
            .Where(w => w.SeasonId == seasonId)
            .OrderBy(w => w.StartDate)
            .ToListAsync();

    public async Task<ReservationWave?> GetByIdAsync(int id) =>
        await _db.ReservationWaves.Include(w => w.Season).FirstOrDefaultAsync(w => w.Id == id);

    public async Task CreateAsync(ReservationWave wave)
    {
        _db.ReservationWaves.Add(wave);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(ReservationWave wave)
    {
        _db.ReservationWaves.Update(wave);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> CanDeleteAsync(int id) =>
        !await _db.Performances.AnyAsync(p => p.ReservationWaveId == id);

    public async Task DeleteAsync(int id)
    {
        var wave = await _db.ReservationWaves.FindAsync(id);
        if (wave != null)
        {
            _db.ReservationWaves.Remove(wave);
            await _db.SaveChangesAsync();
        }
    }
}
