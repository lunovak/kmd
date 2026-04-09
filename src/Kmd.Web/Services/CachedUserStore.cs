using Kmd.Web.Data;
using Microsoft.Extensions.Caching.Memory;

namespace Kmd.Web.Services;

public class CachedUserStore
{
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public CachedUserStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    private static string UserKey(string userId) => $"user:{userId}";
    private static string UserByEmailKey(string email) => $"user:email:{email.ToLowerInvariant()}";

    public ApplicationUser? GetById(string userId) =>
        _cache.TryGetValue(UserKey(userId), out ApplicationUser? user) ? user : null;

    public ApplicationUser? GetByEmail(string email) =>
        _cache.TryGetValue(UserByEmailKey(email), out ApplicationUser? user) ? user : null;

    public void Set(ApplicationUser user)
    {
        var options = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheDuration };
        _cache.Set(UserKey(user.Id), user, options);
        if (user.Email != null)
            _cache.Set(UserByEmailKey(user.Email), user, options);
    }

    public void Invalidate(ApplicationUser user)
    {
        _cache.Remove(UserKey(user.Id));
        if (user.Email != null)
            _cache.Remove(UserByEmailKey(user.Email));
    }
}
