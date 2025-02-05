using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;

namespace iPath.Application.Features.Nodes.Commands;

public record UpdateNodeVisibilityCommand(int NodeId, eNodeVisibility newValue) : IRequest<NodeCommandRespone>
{
}

public class UpdateNodeVisibilityHandler(IPathDbContext ctx) : IRequestHandler<UpdateNodeVisibilityCommand, NodeCommandRespone>
{
    public async Task<NodeCommandRespone> Handle(UpdateNodeVisibilityCommand request, CancellationToken cancellationToken)
    {
        var n = await ctx.Nodes.FindAsync(request.NodeId);
        if (n == null) return new NodeCommandRespone(false, Message: $"Node #{request.NodeId} not found");

        n.Visibility = request.newValue;
        await ctx.SaveChangesAsync();
        return new NodeCommandRespone(true, Item: n);
    }
}



public record UpdateNodesVisibilityCommand(List<int> NodeIds, eNodeVisibility newValue) : IRequest<NodeCommandRespone>
{
}

public class UpdateNodesVisibilityHandler(IPathDbContext ctx) : IRequestHandler<UpdateNodesVisibilityCommand, NodeCommandRespone>
{
    public async Task<NodeCommandRespone> Handle(UpdateNodesVisibilityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            foreach( var id in request.NodeIds )
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