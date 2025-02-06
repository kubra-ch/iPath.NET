using iPath.Data.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features.Nodes.Commands;

public record DeleteNodeCommand (int NodeId) : IRequest<NodeCommandRespone>
{
}

public class DeleteNodeCommandHandler(IDbContextFactory<IPathDbContext> dbFactory) : IRequestHandler<DeleteNodeCommand, NodeCommandRespone>
{
    public async Task<NodeCommandRespone> Handle(DeleteNodeCommand request, CancellationToken cancellationToken)
    {
       using var ctx = await dbFactory.CreateDbContextAsync();
        var n = await ctx.Nodes.FindAsync(request.NodeId);
        if (n == null) return new NodeCommandRespone(false, Message: $"Node #{request.NodeId} not found");

        ctx.Nodes.Remove(n);
        await ctx.SaveChangesAsync();
        return new NodeCommandRespone(true);
    }
}


public record DeleteNodesCommand(List<int> NodeIds) : IRequest<NodeCommandRespone>
{
}

public class DeleteNodesCommandHandler(IDbContextFactory<IPathDbContext> dbFactory) : IRequestHandler<DeleteNodesCommand, NodeCommandRespone>
{
    public async Task<NodeCommandRespone> Handle(DeleteNodesCommand request, CancellationToken cancellationToken)
    {
       using var ctx = await dbFactory.CreateDbContextAsync();
        foreach ( var id in request.NodeIds)
        {
            var n = await ctx.Nodes.FindAsync(id);
            if (n != null) ctx.Nodes.Remove(n);
        }

        await ctx.SaveChangesAsync();
        return new NodeCommandRespone(true);
    }
}