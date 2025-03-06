using iPath.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;


public record GetNodeQuery(int Id) : IRequest<NodeCommandResponse>
{
}

public class GetNodeQueryHandler(IDbContextFactory<NewDB> dbFactory)
    : IRequestHandler<GetNodeQuery, NodeCommandResponse>
{
    public async Task<NodeCommandResponse> Handle(GetNodeQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        var data = await ctx.Nodes
            .Include(n => n.ChildNodes)
            .Include(n => n.Annotations)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == request.Id);

        if (data is null)
            return new NodeCommandResponse(false, $"Node #{request.Id} not found");
        else
            return new NodeCommandResponse(true, node: data);
    }
}