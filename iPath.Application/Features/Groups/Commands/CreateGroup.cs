using iPath.Data;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public class CreateGroupCommand : IRequest<GroupCommandResponse>
{
    [MinLength(4)]
    public string Name;
    [Required]
    public UserDTO Owner;
    public string? Purpose = null;
    public int? CommunityId = null;
}


public class CreateGroupCommandHandler(IDbContextFactory<NewDB> dbFactory, IStringLocalizer T)
    : IRequestHandler<CreateGroupCommand, GroupCommandResponse>
{
    public async Task<GroupCommandResponse> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Name)) return new GroupCommandResponse(false, T["A name must be specified"]);
        if (request.Owner is null) return new GroupCommandResponse(false, T["A Group owner must be specified"]);

        using var ctx = await dbFactory.CreateDbContextAsync(cancellationToken);

        var existing = await ctx.Communities.AsNoTracking().IgnoreQueryFilters().AnyAsync(x => x.Name == request.Name);
        if ( existing )
        {
            return new GroupCommandResponse(false, Message: T["Group with name {0} exists alreay", request.Name]);
        }

        var newEntity = new Group()
        {
            Name = request.Name,
            GroupType = eGroupType.DiscussionGroup,
        };
        newEntity.Settings ??= new();
        newEntity.Settings.Purpose = request.Purpose;


        if (request.CommunityId.HasValue)
        {
            var com = await ctx.Communities.FindAsync(request.CommunityId.Value);
            if( com != null)
            {
                newEntity.Communities.Add(new CommunityGroup()
                {
                    Group = newEntity,
                    Community = com
                });
            }
        }


        await ctx.Groups.AddAsync(newEntity);
        await ctx.SaveChangesAsync(cancellationToken);
        return new GroupCommandResponse(true, group: newEntity);
    }
}