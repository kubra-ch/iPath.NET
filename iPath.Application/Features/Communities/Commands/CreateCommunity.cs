using Azure.Identity;
using iPath.Application.Services;
using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public record  CreateCommunityCommand(string Name, int? OwnerId = null!) : IRequest<CommunityResponse>
{
}


public record CommunityResponse(bool Success, Community? Item = null!, string? Message = default!);



public class CreateCommunityCommandHandler(IDbContextFactory<IPathDbContext> dbFactory, ISessionStateService sessState)
    : IRequestHandler<CreateCommunityCommand, CommunityResponse>
{
    public async Task<CommunityResponse> Handle(CreateCommunityCommand request, CancellationToken cancellationToken)
    {
       using var ctx = await dbFactory.CreateDbContextAsync();

        // check that neither Communityname or email is used
        if ( await ctx.Communities.AnyAsync (u => u.Name == request.Name))
        {
            return new CommunityResponse(false, Message: $"Community {request.Name} already exists");
        }

        try
        {
            User owner = null!;
            if (request.OwnerId.HasValue) owner = await ctx.Users.FindAsync(request.OwnerId.Value);
            if ( owner == null && sessState.SessionUserId.HasValue )
            {
                // fallback to sesison user
                owner = await ctx.Users.FindAsync(sessState.SessionUserId.Value);
            }

            if (owner == null) return new CommunityResponse(false, Message: "a community owner must be sopecified"); 

            Community item = new Community();
            item.Name = request.Name;
            item.Owner = owner;

            ctx.Communities.Add(item);
            await ctx.SaveChangesAsync();
            return new CommunityResponse(true, item);
        }
        catch(Exception ex)
        {
            return new CommunityResponse(false, Message: ex.Message);
        }
    }
}