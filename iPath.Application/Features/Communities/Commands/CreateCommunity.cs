using Azure.Identity;
using iPath.Application.Services;
using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public record  CreateCommunityCommand(string Name, int? OwnerId = null!) : IRequest<CommunityCommandResponse>
{
}



public class CreateCommunityCommandHandler(IDbContextFactory<IPathDbContext> dbFactory, ISessionStateService sessState)
    : IRequestHandler<CreateCommunityCommand, CommunityCommandResponse>
{
    public async Task<CommunityCommandResponse> Handle(CreateCommunityCommand request, CancellationToken cancellationToken)
    {
       using var ctx = await dbFactory.CreateDbContextAsync();

        // check that neither Communityname or email is used
        if ( await ctx.Communities.AnyAsync (u => u.Name == request.Name))
        {
            return new CommunityCommandResponse(false, Message: $"Community {request.Name} already exists");
        }

        User owner = null!;
        if (request.OwnerId.HasValue) owner = await ctx.Users.FindAsync(request.OwnerId.Value);
        if ( owner == null && sessState.SessionUserId.HasValue )
        {
            // fallback to sesison user
            owner = await ctx.Users.FindAsync(sessState.SessionUserId.Value);
        }

        if (owner == null) return new CommunityCommandResponse(false, Message: "a community owner must be sopecified"); 

        Community item = new Community();
        item.Name = request.Name;
        item.Owner = owner;

        ctx.Communities.Add(item);
        await ctx.SaveChangesAsync();
        return new CommunityCommandResponse(true, Data: item);
    }
}