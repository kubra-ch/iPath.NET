using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class UpdateUserMembershipCommand : IRequest<UserCommandResponse>
{
    public int UserId { get; set; }
    public List<UserGroupMemberDto> Data = new();
}

public record UserGroupMemberDto(int GroupId, eMemberRole Role);



public class UpdateUserMembershipCommandHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<UpdateUserMembershipCommand, UserCommandResponse>
{
    public async Task<UserCommandResponse> Handle(UpdateUserMembershipCommand request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        var set = ctx.Set<GroupMember>();
        var list = await set.Where(m => m.UserId == request.UserId).ToListAsync();

        // remove those set to None
        foreach (var entity in list)
        {
            if (!request.Data.Any(dto => dto.GroupId == entity.GroupId))
            {
                set.Remove(entity);
            }
        }

        // update and add new
        foreach (var dto in request.Data)
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

        return new UserCommandResponse(true);
    }
}