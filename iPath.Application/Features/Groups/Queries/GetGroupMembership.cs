using iPath.Application.Querying;
using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public record GetGroupMembershipQuery(int? UserId = null!, int? GroupId = null!) : IRequest<GetGroupMembershipResponse>;



public record GetGroupMembershipResponse(bool Success, string? Message = default!, List<GroupMember> Data = null!)
    : BaseResponseT<List<GroupMember>>(Success, Message, Data);



public class GetGroupMembershipQueryHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<GetGroupMembershipQuery, GetGroupMembershipResponse>
{
    public async Task<GetGroupMembershipResponse> Handle(GetGroupMembershipQuery request, CancellationToken cancellationToken)
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

        return new GetGroupMembershipResponse(true, Data: await q.ToListAsync());
    }
}