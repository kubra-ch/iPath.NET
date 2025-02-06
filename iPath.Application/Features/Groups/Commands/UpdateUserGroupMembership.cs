using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class UpdateUserGroupMembershipCommand : IRequest<UpdateUserGroupMembershipResponse>
{
    public int UserId { get; set; }
    public int GroupId { get; set; }   
    public eMemberRole Role { get; set; }
}


public record UpdateUserGroupMembershipResponse(bool Success, string? Message = default!);


public class UpdateUserGroupMembershipCommandHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<UpdateUserGroupMembershipCommand, UpdateUserGroupMembershipResponse>
{
    public async Task<UpdateUserGroupMembershipResponse> Handle(UpdateUserGroupMembershipCommand request, CancellationToken cancellationToken)
    {
        try
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

            return new UpdateUserGroupMembershipResponse(true);
        }
        catch(Exception ex)
        {
            return new UpdateUserGroupMembershipResponse(false, Message: (ex.InnerException is null ? ex.Message : ex.InnerException.Message));
        }
    }
}