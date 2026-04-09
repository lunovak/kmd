using Kmd.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Kmd.Web.Services;

public class TheatreService
{
    private readonly ApplicationDbContext _db;

    public TheatreService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<Theatre>> GetAllAsync() =>
        await _db.Theatres.OrderBy(t => t.Name).ToListAsync();

    public async Task<Theatre?> GetByIdAsync(int id) =>
        await _db.Theatres.FindAsync(id);

    public async Task CreateAsync(Theatre theatre)
    {
        _db.Theatres.Add(theatre);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Theatre theatre)
    {
        _db.Theatres.Update(theatre);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> CanDeleteAsync(int id) =>
        !await _db.Plays.AnyAsync(p => p.TheatreId == id)
        && !await _db.Performances.AnyAsync(p => p.TheatreId == id);

    public async Task DeleteAsync(int id)
    {
        var theatre = await _db.Theatres.FindAsync(id);
        if (theatre != null)
        {
            _db.Theatres.Remove(theatre);
            await _db.SaveChangesAsync();
        }
    }
}
