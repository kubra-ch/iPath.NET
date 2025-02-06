using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class UpdateGroupCommand : IRequest<UpdateGroupResponse>
{
    public Group Item { get; set; }
}


public record UpdateGroupResponse(bool Success, Group? Item = null!, string? Message = default!);



public class UpdateGroupCommandHandler(IDbContextFactory<IPathDbContext> dbFactory, IPasswordHasher hasher)
    : IRequestHandler<UpdateGroupCommand, UpdateGroupResponse>
{
    public async Task<UpdateGroupResponse> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        try
        {
           using var ctx = await dbFactory.CreateDbContextAsync();

            // get the group from DB
            var item = await ctx.Groups.FindAsync(request.Item.Id);

            item.Name = request.Item.Name;
            item.Purpose = request.Item.Purpose;
            item.OwnerId = request.Item.OwnerId;
            item.CommunityId = request.Item.CommunityId;
            item.Settings = request.Item.Settings;
            item.Visibility = request.Item.Visibility;

            ctx.Groups.Update(item);
            await ctx.SaveChangesAsync();
            return new UpdateGroupResponse(true, item);
        }
        catch(Exception ex)
        {
            return new UpdateGroupResponse(false, Message: (ex.InnerException is null ? ex.Message : ex.InnerException.Message));
        }
    }
}