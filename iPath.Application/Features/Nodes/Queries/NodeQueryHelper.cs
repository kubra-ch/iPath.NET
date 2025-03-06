using iPath.Data;
using iPath.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

internal static class NodeQueryHelper
{
    public static IQueryable<Node> PrepareNodeQuery(this NewDB ctx, int? UserId = null, int? GroupId=null, bool RecentlyVisited = false, string? SearchString = null!)
    {
        var q = ctx.Nodes
            .Where(n => n.GroupId.HasValue)
            .Include(n => n.ChildNodes)
            .Include(n => n.Annotations)
            .Include(n => n.Owner)
            .AsNoTracking().AsQueryable();

        if (RecentlyVisited && UserId.HasValue)
        { 
            q = q.Where(n => n.Visits.Any(v => v.UserId == UserId.Value && v.Date > DateTime.UtcNow.AddMonths(-1)));
        } 
        else if (GroupId.HasValue)
        {
            q = q.Where(n => n.GroupId == GroupId.Value);
        }
        else if (UserId.HasValue)
        {
            q = q.Where(n => n.OwnerId == UserId.Value);
        }
        if (!string.IsNullOrEmpty(SearchString))
        {
            q = q.Where(g => EF.Functions.ILike(g.Description.Title, $"%{SearchString}%") ||
                             EF.Functions.ILike(g.Description.Subtitle, $"%{SearchString}%") ||
                             EF.Functions.ILike(g.Description.Text, $"%{SearchString}%") ||
                             g.Annotations.Any(a => EF.Functions.ILike(a.Text, $"%{SearchString}%"))
            );
        }

        return q;
    }


    public static async Task<bool> IsUserInGroup(this NewDB ctx, int UserId, int GroupId)
    {
        // Group Member?
        if (await ctx.Set<GroupMember>().AsNoTracking().AnyAsync(m => m.User.IsActive && m.UserId == UserId && m.GroupId == GroupId && m.Role != eMemberRole.Inactive))
            return true;

        // Admin?
        if (await ctx.Users.AsNoTracking().AnyAsync(u => u.IsActive && u.Id == UserId && u.Roles.Any(r => r.Name == UserRole.Admin.Name)))
            return true;

        return false;
    }
}
