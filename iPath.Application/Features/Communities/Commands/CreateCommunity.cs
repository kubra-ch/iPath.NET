using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public class CreateCommunityCommand : IRequest<CreateCommunityResponse>
{
    [MinLength(3)] 
    public string Name { get; set; }
}


public record CreateCommunityResponse(bool Success, Community? Item = null!, string? Message = default!);



public class CreateCommunityCommandHandler(IPathDbContext ctx, IPasswordHasher hasher)
    : IRequestHandler<CreateCommunityCommand, CreateCommunityResponse>
{
    public async Task<CreateCommunityResponse> Handle(CreateCommunityCommand request, CancellationToken cancellationToken)
    {
        // check that neither Communityname or email is used
        if( await ctx.Communities.AnyAsync (u => u.Name == request.Name))
        {
            return new CreateCommunityResponse(false, Message: $"Community {request.Name} already exists");
        }

        try
        {
            Community item = new Community
            {
                Name = request.Name
            };
            ctx.Communities.Add(item);
            await ctx.SaveChangesAsync();
            return new CreateCommunityResponse(true, item);
        }
        catch(Exception ex)
        {
            return new CreateCommunityResponse(false, Message: ex.Message);
        }
    }
}