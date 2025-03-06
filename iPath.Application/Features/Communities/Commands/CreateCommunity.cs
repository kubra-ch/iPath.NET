using iPath.Data;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public class CreateCommunityCommand : IRequest<CommunityCommandResponse>
{
    [MinLength(4)]
    public string Name;
    [Required]
    public UserDTO Owner;
    public string? Description = null;
}


public class CreateCommunityCommandHandler(IDbContextFactory<NewDB> dbFactory, IStringLocalizer T)
    : IRequestHandler<CreateCommunityCommand, CommunityCommandResponse>
{
    public async Task<CommunityCommandResponse> Handle(CreateCommunityCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Name)) return new CommunityCommandResponse(false, T["A name must be specified"]);
        if (request.Owner is null) return new CommunityCommandResponse(false, T["A community owner must be specified"]);

        using var ctx = await dbFactory.CreateDbContextAsync(cancellationToken);

        var existing = await ctx.Communities.AsNoTracking().IgnoreQueryFilters().AnyAsync(x => x.Name == request.Name);
        if ( existing )
        {
            return new CommunityCommandResponse(false, Message: T["Community with name {0} exists alreay", request.Name]);
        }

        var newEntity = new Community()
        {
            Name = request.Name,
            OwnerId = request.Owner.UserId,
            Description = request.Description
        };

        await ctx.Communities.AddAsync(newEntity);
        await ctx.SaveChangesAsync(cancellationToken);
        return new CommunityCommandResponse(true, community: newEntity);
    }
}