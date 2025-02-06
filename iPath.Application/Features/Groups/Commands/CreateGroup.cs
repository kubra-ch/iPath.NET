using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public class CreateGroupCommand : IRequest<CreateGroupResponse>
{
    [MinLength(3)]
    public string Name { get; set; }
    public string? Purpose { get; set; }
    public int? OwnerId { get; set; }
    public int? CommunityId { get; set; }
}


public record CreateGroupResponse(bool Success, Group? Item = null!, string? Message = default!);



public class CreateGroupCommandHandler(IDbContextFactory<IPathDbContext> dbFactory, IPasswordHasher hasher)
    : IRequestHandler<CreateGroupCommand, CreateGroupResponse>
{
    public async Task<CreateGroupResponse> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();

        // check that neither Groupname or email is used
        if ( await ctx.Groups.AnyAsync (u => u.Name == request.Name))
        {
            return new CreateGroupResponse(false, Message: $"Group {request.Name} already exists");
        }

        try
        {
            Group item = new Group
            {
                Name = request.Name,
                Purpose = request.Purpose,
                OwnerId = request.OwnerId,
                CommunityId = request.CommunityId
            };
            ctx.Groups.Add(item);
            await ctx.SaveChangesAsync();
            return new CreateGroupResponse(true, item);
        }
        catch(Exception ex)
        {
            return new CreateGroupResponse(false, Message: (ex.InnerException is null ? ex.Message : ex.InnerException.Message));
        }
    }
}