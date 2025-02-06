using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public class UpdateNodeCommand : IRequest<NodeCommandRespone>
{
    public int Id { get; set; }

    [MinLength(3)]
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string? Description { get; set; }
    public eNodeStatus Status { get; set; }
    public eNodeVisibility Visibility { get; set; }
}


public class UpdateNodeCommandHandler(IDbContextFactory<IPathDbContext> dbFactory, IPasswordHasher hasher)
    : IRequestHandler<UpdateNodeCommand, NodeCommandRespone>
{
    public async Task<NodeCommandRespone> Handle(UpdateNodeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using var ctx = await dbFactory.CreateDbContextAsync();

            var node = await ctx.Nodes.FindAsync(request.Id);
            if (node == null) return new NodeCommandRespone(false, Message: $"Node #{request.Id} not found");

            node.Title = request.Title;
            node.SubTitle = request.SubTitle;
            node.Description = request.Description;
            node.Status = request.Status;
            node.Visibility = request.Visibility;
            node.ModifiedOn = DateTime.UtcNow;

            await ctx.SaveChangesAsync();
            return new NodeCommandRespone(true, node);
        }
        catch(Exception ex)
        {
            return new NodeCommandRespone(false, Message: (ex.InnerException is null ? ex.Message : ex.InnerException.Message));
        }
    }
}