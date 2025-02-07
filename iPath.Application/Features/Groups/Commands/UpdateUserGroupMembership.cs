using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public record UpdateUserGroupMembershipCommand(
     int UserId,
     int GroupId,
     eMemberRole Role
) : IRequest<GroupCommandResponse>;


public class UpdateUserGroupMembershipCommandHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<UpdateUserGroupMembershipCommand, GroupCommandResponse>
{
    public async Task<GroupCommandResponse> Handle(UpdateUserGroupMembershipCommand request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        var m = await ctx.Set<GroupMember>().FirstOrDefaultAsync(gm => gm.UserId == request.UserId && gm.GroupId == request.GroupId);
        if (m == null)
        {
            m = new GroupMember()
            {
                UserId = request.UserId,
                GroupId = request.GroupId,
            };
            await ctx.Set<GroupMember>().AddAsync(m);
        }
        m.Role = request.Role;
        await ctx.SaveChangesAsync();

        return new GroupCommandResponse(true);
    }
}