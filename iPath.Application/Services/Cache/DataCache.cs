using iPath.Application.Features;
using iPath.Data;
using iPath.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace iPath.Application.Services.Cache;

public interface IDataCache
{
    Task<UserProfile> GetProfileAsync(int id);
    Task<GroupDTO> GetGroupDtoAsync(int id);
}


public class DataCache(IMemoryCache cache, IDbContextFactory<NewDB> fct) : IDataCache
{

    public async Task<UserProfile> GetProfileAsync(int id)
    {
        var key = $"user_{id}";
        return await cache.GetOrCreateAsync(key,
            entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromMinutes(5));
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
                return fetchProfile();
            });

        async Task<UserProfile> fetchProfile()
        {
            using var ctx = await fct.CreateDbContextAsync();
            var user = await ctx.Users.FindAsync(id);
            return user?.Profile;
        }
    }

    public async Task<GroupDTO> GetGroupDtoAsync(int id)
    {
        var key = $"group_{id}";
        return await cache.GetOrCreateAsync(key,
            entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromMinutes(5));
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
                return fetchGroup();
            });

        async Task<GroupDTO> fetchGroup()
        {
            using var ctx = await fct.CreateDbContextAsync();
            var g = await ctx.Groups.FindAsync(id);
            return new GroupDTO { Id = g.Id, Name = g.Name };
        }
    }
}
