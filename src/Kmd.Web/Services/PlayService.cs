using Kmd.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Kmd.Web.Services;

public class PlayService
{
    private readonly ApplicationDbContext _db;

    public PlayService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<Play>> GetAllAsync(int? theatreId = null)
    {
        var query = _db.Plays.Include(p => p.Theatre).AsQueryable();
        if (theatreId.HasValue)
            query = query.Where(p => p.TheatreId == theatreId.Value);
        return await query.OrderBy(p => p.Theatre.Name).ThenBy(p => p.Name).ToListAsync();
    }

    public async Task<Play?> GetByIdAsync(int id) =>
        await _db.Plays.Include(p => p.Theatre).FirstOrDefaultAsync(p => p.Id == id);

    public async Task CreateAsync(Play play)
    {
        _db.Plays.Add(play);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Play play)
    {
        _db.Plays.Update(play);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> CanDeleteAsync(int id) =>
        !await _db.Performances.AnyAsync(p => p.PlayId == id);

    public async Task DeleteAsync(int id)
    {
        var play = await _db.Plays.FindAsync(id);
        if (play != null)
        {
            _db.Plays.Remove(play);
            await _db.SaveChangesAsync();
        }
    }
}
