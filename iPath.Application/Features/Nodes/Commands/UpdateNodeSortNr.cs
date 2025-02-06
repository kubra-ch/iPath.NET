using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public record UpdateNodeSortNrSortNrCommand(int NodeId, int SortNr) : IRequest<NodeCommandRespone>
{
}


public class UpdateNodeSortNrSortNrCommandHandler(IDbContextFactory<IPathDbContext> dbFactory, IPasswordHasher hasher)
    : IRequestHandler<UpdateNodeSortNrSortNrCommand, NodeCommandRespone>
{
    public async Task<NodeCommandRespone> Handle(UpdateNodeSortNrSortNrCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using var ctx = await dbFactory.CreateDbContextAsync();

            var node = await ctx.Nodes.FindAsync(request.NodeId);
            if (node == null) return new NodeCommandRespone(false, Message: $"Node #{request.NodeId} not found");

            node.SortNr = request.SortNr;

            await ctx.SaveChangesAsync();
            return new NodeCommandRespone(true, node);
        }
        catch(Exception ex)
        {
            return new NodeCommandRespone(false, Message: (ex.InnerException is null ? ex.Message : ex.InnerException.Message));
        }
    }
}

public record UpdateNodesSortNrSortNrCommand(List<(int NodeId, int SortNr)> newOrder) : IRequest<NodeCommandRespone>
{
}


public class UpdateNodesSortNrSortNrCommandHandler(IDbContextFactory<IPathDbContext> dbFactory, IPasswordHasher hasher)
    : IRequestHandler<UpdateNodesSortNrSortNrCommand, NodeCommandRespone>
{
    public async Task<NodeCommandRespone> Handle(UpdateNodesSortNrSortNrCommand request, CancellationToken cancellationToken)
    {
        try
        {
           using var ctx = await dbFactory.CreateDbContextAsync();

            foreach (var item in request.newOrder)
            {
                var node = await ctx.Nodes.FindAsync(item.NodeId);
                if (node != null) node.SortNr = item.SortNr;
            }

            await ctx.SaveChangesAsync();
            return new NodeCommandRespone(true, Item: null, Message: $"{request.newOrder.Count} nodes updated");
        }
        catch (Exception ex)
        {
            return new NodeCommandRespone(false, Message: (ex.InnerException is null ? ex.Message : ex.InnerException.Message));
        }
    }
}