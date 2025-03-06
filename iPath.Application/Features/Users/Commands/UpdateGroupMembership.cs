using iPath.Data;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;


public record GroupMemberShipDto (int GroupId, eMemberRole Role);


public record UpdateGroupMembershipCommand(int UserId, GroupMemberShipDto[] Membership) : IRequest<BaseResponse>;


public class UpdateGroupMembershipCommandHandler(IDbContextFactory<NewDB> fct)
    : IRequestHandler<UpdateGroupMembershipCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(UpdateGroupMembershipCommand request, CancellationToken cancellationToken)
    {
        using var ctx = await fct.CreateDbContextAsync();
        var set = ctx.Set<GroupMember>();

        // reload from DB
        var list = await set.Where(m => m.UserId == request.UserId).ToListAsync();

        // remove those set to None
        foreach (var entity in list)
        {
            if (!request.Membership.Any(dto => dto.GroupId == entity.GroupId))
            {
                set.Remove(entity);
            }
        }

        // update and add new
        foreach (var dto in request.Membership)
        {
            var entity = list.FirstOrDefault(m => m.GroupId == dto.GroupId);
            if (entity == null)
            {
                entity = new GroupMember()
                {
                    UserId = request.UserId,
                    GroupId = dto.GroupId,
                };
                await set.AddAsync(entity);
            }
            entity.Role = dto.Role;
        }

        await ctx.SaveChangesAsync();

        return new BaseResponse(true, "");
    }
}