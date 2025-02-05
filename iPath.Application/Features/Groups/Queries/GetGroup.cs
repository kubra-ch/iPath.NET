using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public record GetGroupQuery(int GroupId) : IRequest<GetGroupResponse>
{
}

public record GetGroupResponse(bool Success, Group Item, string? Message = default!);


public class GetGroupQueryHandler(IPathDbContext ctx) : IRequestHandler<GetGroupQuery, GetGroupResponse>
{
    public async Task<GetGroupResponse> Handle(GetGroupQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var e = await ctx.Groups
                .Include(g => g.Community)
                .Include(g => g.Owner)
                .FirstOrDefaultAsync(g => g.Id == request.GroupId);

            if (e != null) return new GetGroupResponse(true, e);

            throw new Exception($"Group with Id={request.GroupId} not found");
        }
        catch (Exception ex)
        {
            return new GetGroupResponse(false, null, ex.InnerException is null ? ex.Message : ex.InnerException.Message);
        }
    }
}