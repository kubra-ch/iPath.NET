using iPath.Data;
using iPath.Data.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Security.Claims;

namespace iPath.Application.Authentication;


public interface IAuthManager
{
    Task<UserProfile> GetProfileAync();
    void Init(ClaimsPrincipal claimsPrincipal);
    bool IsAdmin();
    bool IsModerator();
    Task<bool> IsModerator(int groupId);
}


public class AuthManager : IAuthManager
{
    private readonly IDbContextFactory<NewDB> fct;
    public ClaimsPrincipal principal {get; private set;}
    private UserProfile profile;

    public AuthManager(IDbContextFactory<NewDB> fct)
    {
        this.fct = fct;
    }

    public void Init(ClaimsPrincipal claimsPrincipal)
    {
        principal = claimsPrincipal;
    }

    public async Task<UserProfile> GetProfileAync()
    {
        if (profile == null && principal != null)
        {
            using var ctx = await fct.CreateDbContextAsync();
            var usr = await ctx.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == principal.Identity.Name);
            profile = (usr != null ? usr.Profile : new UserProfile() { Username = "anonymous" });
        }
        return profile;
    }

    public bool IsAdmin()
    {
        if( principal != null )
        {
            return principal.Claims.Any(r => r.Type == ClaimTypes.Role && r.Value == UserRole.Admin.Name);
        }
        return false;
    }

    public bool IsModerator()
    {
        if (principal != null)
        {
            return principal.Claims.Any(r => r.Type == ClaimTypes.Role && r.Value == UserRole.Moderator.Name);
        }
        return false;
    }


    private HashSet<int> ModeratedGroups = null!;
    public async Task<bool> IsModerator(int groupId)
    {
        if( ModeratedGroups is null)
        {
            var userid = (await GetProfileAync()).UserId;
            using var ctx = await fct.CreateDbContextAsync();
            ModeratedGroups = await ctx.Set<GroupMember>().AsNoTracking()
                .Where(m => m.UserId == userid && m.Role.HasFlag(eMemberRole.Moderator))
                .Select(m => m.GroupId).ToHashSetAsync();
            ModeratedGroups ??= new();
        }

        return ModeratedGroups.Contains(groupId);
    }


    private HashSet<int> ActiveGroups = null!;
    public async Task<bool> IsMember(int groupId)
    {
        if (IsAdmin()) return true;

        if (ActiveGroups is null)
        {
            var userid = (await GetProfileAync()).UserId;
            using var ctx = await fct.CreateDbContextAsync();
            ActiveGroups = await ctx.Set<GroupMember>().AsNoTracking()
                .Where(m => m.UserId == userid && m.Role != eMemberRole.Inactive)
                .Select(m => m.GroupId).ToHashSetAsync();
            ActiveGroups ??= new();
        }

        return ActiveGroups.Contains(groupId);
    }
}





public class AuthManagerBlazor
{
    private readonly AuthenticationStateProvider asp;
    private readonly IAuthManager authManager;

    public AuthManagerBlazor(AuthenticationStateProvider authStateProvider, IAuthManager am)
    {
        asp = authStateProvider;
        asp.AuthenticationStateChanged += AuthenticationStateChangedHandler;
        authManager = am;
    }

    private async void AuthenticationStateChangedHandler(Task<AuthenticationState> e)
    {
        authManager.Init((await e).User);
    }


    public async Task<UserProfile> GetProfileAync()
    {
        return await authManager.GetProfileAync();
    }
}
