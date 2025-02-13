using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public record UpdateGroupCommand(GroupDto Item) : IRequest<GroupCommandResponse>
{
}


public class UpdateGroupCommandHandler(IDbContextFactory<IPathDbContext> dbFactory, IPasswordHasher hasher)
    : IRequestHandler<UpdateGroupCommand, GroupCommandResponse>
{
    public async Task<GroupCommandResponse> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();

        // get the group from DB
        var item = await ctx.Groups.FindAsync(request.Item.Id);

        item.Name = request.Item.Name;
        item.Purpose = request.Item.Purpose;
        item.OwnerId = request.Item.Owner?.Id;
        item.CommunityId = request.Item.Community?.Id;
        item.Visibility = request.Item.Visibility;

        ctx.Groups.Update(item);
        await ctx.SaveChangesAsync();
        return new GroupCommandResponse(true, Data: item.ToDto());
    }
}