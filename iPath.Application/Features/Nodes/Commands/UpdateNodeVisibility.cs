using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features.Nodes.Commands;

public record UpdateNodeVisibilityCommand(int NodeId, eNodeVisibility newValue) : IRequest<NodeCommandRespone>
{
}

public class UpdateNodeVisibilityHandler(IDbContextFactory<IPathDbContext> dbFactory) : IRequestHandler<UpdateNodeVisibilityCommand, NodeCommandRespone>
{
    public async Task<NodeCommandRespone> Handle(UpdateNodeVisibilityCommand request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();

        var n = await ctx.Nodes.FindAsync(request.NodeId);
        if (n == null) return new NodeCommandRespone(false, Message: $"Node #{request.NodeId} not found");

        n.Visibility = request.newValue;
        await ctx.SaveChangesAsync();
        return new NodeCommandRespone(true, Data: n);
    }
}



public record UpdateNodesVisibilityCommand(List<int> NodeIds, eNodeVisibility newValue) : IRequest<NodeCommandRespone>
{
}

public class UpdateNodesVisibilityHandler(IDbContextFactory<IPathDbContext> dbFactory) : IRequestHandler<UpdateNodesVisibilityCommand, NodeCommandRespone>
{
    public async Task<NodeCommandRespone> Handle(UpdateNodesVisibilityCommand request, CancellationToken cancellationToken)
    {
        try
        {
           using var ctx = await dbFactory.CreateDbContextAsync();

            foreach ( var id in request.NodeIds )
            {
                var n = await ctx.Nodes.FindAsync(id);
                if( n != null ) n.Visibility = request.newValue;
            }

            await ctx.SaveChangesAsync();
            return new NodeCommandRespone(true);
        }
        catch (Exception ex)
        {
            return new NodeCommandRespone(false, Message: ex.InnerException is null ? ex.Message : ex.InnerException.Message);
        }
    }
}