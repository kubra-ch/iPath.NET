using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public record CreateGroupCommand( 
    [MinLength(3)]
     string Name,
     string? Purpose,
     int? OwnerId,
     int? CommunityId
) : IRequest<GroupCommandResponse>;


public class CreateGroupCommandHandler(IDbContextFactory<IPathDbContext> dbFactory, IPasswordHasher hasher)
    : IRequestHandler<CreateGroupCommand, GroupCommandResponse>
{
    public async Task<GroupCommandResponse> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();

        // check that neither Groupname or email is used
        if (await ctx.Groups.AnyAsync(u => u.Name == request.Name))
        {
            return new GroupCommandResponse(false, Message: $"Group {request.Name} already exists");
        }

        Group item = new Group
        {
            Name = request.Name,
            Purpose = request.Purpose,
            OwnerId = request.OwnerId,
            CommunityId = request.CommunityId
        };
        ctx.Groups.Add(item);
        await ctx.SaveChangesAsync();
        return new GroupCommandResponse(true, Data: item.ToDto());
    }
}