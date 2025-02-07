using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public record GetGroupQuery(int GroupId) : IRequest<GetGroupResponse>
{
}

public record GetGroupResponse(bool Success, string? Message = default!, Group Data = null!)
    : BaseResponseT<Group>(Success, Message, Data);


public class GetGroupQueryHandler(IDbContextFactory<IPathDbContext> dbFactory) : IRequestHandler<GetGroupQuery, GetGroupResponse>
{
    public async Task<GetGroupResponse> Handle(GetGroupQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        var e = await ctx.Groups
            .Include(g => g.Community)
            .Include(g => g.Owner)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == request.GroupId);

        if (e != null) return new GetGroupResponse(true, Data: e);

        throw new Exception($"Group with Id={request.GroupId} not found");
    }
}