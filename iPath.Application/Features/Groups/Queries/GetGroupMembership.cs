using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class GetGroupMembershipQuery : IRequest<List<GroupMember>>
{
    public int? UserId { get; set; }
    public int? GroupId { get; set; }
}

public class GetGroupMembershipQueryHandler(IDbContextFactory<IPathDbContext> dbFactory) : IRequestHandler<GetGroupMembershipQuery, List<GroupMember>>
{
    public async Task<List<GroupMember>> Handle(GetGroupMembershipQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        var q = ctx.Set<GroupMember>().AsNoTracking().AsQueryable();
        
        if( request.UserId.HasValue)
        {
            q = q.Where(u => u.UserId == request.UserId);
        }
        if( request.GroupId.HasValue)
        {
            q = q.Where(u => u.GroupId == request.GroupId);
        }

        return await q.ToListAsync();
    }
}