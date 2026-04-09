using Kmd.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Kmd.Web.Services;

public class SeasonService
{
    private readonly ApplicationDbContext _db;

    public SeasonService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<Season>> GetAllAsync() =>
        await _db.Seasons.OrderByDescending(s => s.StartDate).ToListAsync();

    public async Task<Season?> GetByIdAsync(int id) =>
        await _db.Seasons.FindAsync(id);

    public async Task CreateAsync(Season season)
    {
        _db.Seasons.Add(season);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Season season)
    {
        _db.Seasons.Update(season);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> CanDeleteAsync(int id) =>
        !await _db.ReservationWaves.AnyAsync(w => w.SeasonId == id)
        && !await _db.Subscriptions.AnyAsync(s => s.SeasonId == id);

    public async Task DeleteAsync(int id)
    {
        var season = await _db.Seasons.FindAsync(id);
        if (season != null)
        {
            _db.Seasons.Remove(season);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<int?> GetLatestSeasonIdAsync()
    {
        var latest = await _db.Seasons.OrderByDescending(s => s.StartDate).FirstOrDefaultAsync();
        return latest?.Id;
    }
}
