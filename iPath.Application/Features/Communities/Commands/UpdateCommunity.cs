using iPath.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public class UpdateCommunityCommand : IRequest<CommunityCommandResponse>
{
    public int Id { get; init; }

    [MinLength(4)]
    public string Name;
    [Required]
    public int? OwnerId;

    public string? Description = null;
}


public class UpdateCommunityCommandHandler(IDbContextFactory<NewDB> dbFactory, IStringLocalizer T)
    : IRequestHandler<UpdateCommunityCommand, CommunityCommandResponse>
{
    public async Task<CommunityCommandResponse> Handle(UpdateCommunityCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Name)) return new CommunityCommandResponse(false, T["A name must be specified"]);
        if (!request.OwnerId.HasValue) return new CommunityCommandResponse(false, T["A community owner must be specified"]);

        using var ctx = await dbFactory.CreateDbContextAsync(cancellationToken);

        var existing = await ctx.Communities.AsNoTracking().IgnoreQueryFilters().AnyAsync(x => x.Id != request.Id && x.Name == request.Name);
        if ( existing )
        {
            return new CommunityCommandResponse(false, Message: T["Community with name {0} exists alreay", request.Name]);
        }

        var e = await ctx.Communities.FindAsync(request.Id);
        if( e is null) return new CommunityCommandResponse(false, T["Community {0} not found", request.Id]);

        e.Name = request.Name;
        e.Description  = request.Description;
        e.OwnerId = request.OwnerId.Value;

        ctx.Communities.Update(e);
        await ctx.SaveChangesAsync(cancellationToken);
        return new CommunityCommandResponse(true, community: e);
    }
}