using iPath.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public class UpdateGroupCommand : IRequest<GroupCommandResponse>
{
    public int Id { get; init; }

    [MinLength(4)]
    public string Name;
    [Required]
    public int? OwnerId;

    public string? Purpose = null;
}


public class UpdateGroupCommandHandler(IDbContextFactory<NewDB> dbFactory, IStringLocalizer T)
    : IRequestHandler<UpdateGroupCommand, GroupCommandResponse>
{
    public async Task<GroupCommandResponse> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Name)) return new GroupCommandResponse(false, T["A name must be specified"]);
        if (!request.OwnerId.HasValue) return new GroupCommandResponse(false, T["A Group owner must be specified"]);

        using var ctx = await dbFactory.CreateDbContextAsync(cancellationToken);

        var existing = await ctx.Groups.AsNoTracking().IgnoreQueryFilters().AnyAsync(x => x.Id != request.Id && x.Name == request.Name);
        if ( existing )
        {
            return new GroupCommandResponse(false, Message: T["Group with name {0} exists alreay", request.Name]);
        }

        var e = await ctx.Groups.FindAsync(request.Id);
        if( e is null) return new GroupCommandResponse(false, T["Group {0} not found", request.Id]);

        e.Name = request.Name;
        e.OwnerId = request.OwnerId.Value;
        e.Settings.Purpose = request.Purpose;

        ctx.Groups.Update(e);
        await ctx.SaveChangesAsync(cancellationToken);
        return new GroupCommandResponse(true, group: e);
    }
}