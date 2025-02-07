using iPath.Application.Querying;
using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;


public record GetNodeResponse(bool Success, string? Message = default!, Node Data = null!)
    : BaseResponseT<Node>(Success, Message, Data);

public record GetNodeQuery(int Id) : IRequest<GetNodeResponse>
{
}

public class GetNodeQueryHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<GetNodeQuery, GetNodeResponse>
{
    public async Task<GetNodeResponse> Handle(GetNodeQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        var data = await ctx.Nodes
            .Include(n => n.Owner)
            .Include(n => n.File)
            .Include(n => n.ChildNodes)
                .ThenInclude(c =>c.Owner)
            .Include(n => n.ChildNodes)
                .ThenInclude(c => c.File)
            .Include(n => n.Annotations)
                .ThenInclude(c => c.Owner)
            .Include(n => n.Fields)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == request.Id);

        if (data is null)
            return new GetNodeResponse (false, $"Node #{request.Id} not found");
        else
            return new GetNodeResponse(true, Data: data);
    }
}