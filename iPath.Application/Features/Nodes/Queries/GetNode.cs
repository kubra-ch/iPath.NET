using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public class GetNodeQuery : IRequest<Node>
{
    public int Id { get; set; }
}

public class GetNodeQueryHandler(IDbContextFactory<IPathDbContext> dbFactory) : IRequestHandler<GetNodeQuery, Node>
{
    public async Task<Node> Handle(GetNodeQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        return await ctx.Nodes
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
    }
}