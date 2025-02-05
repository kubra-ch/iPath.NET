using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public class CreateNodeCommand : IRequest<NodeCommandRespone>
{
    [MinLength(3)]
    public string Title { get; set; }
    public string? Description { get; set; }
    public NodeType NodeType { get; set; }
    public int? GroupId { get; set; }
    public int? ParentNodeId { get; set; }
    public int? TopNodetId { get; set; }
    public int? OwnerId { get; set; }
}


public record NodeCommandRespone(bool Success, Node? Item = null!, string? Message = default!);



public class CreateNodeCommandHandler(IPathDbContext ctx, IPasswordHasher hasher)
    : IRequestHandler<CreateNodeCommand, NodeCommandRespone>
{
    public async Task<NodeCommandRespone> Handle(CreateNodeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Node item = new Node
            {
                Title = request.Title,
                Description = request.Description,
                OwnerId = request.OwnerId,
                GroupId = request.GroupId,
                ParentNodeId = request.ParentNodeId,
                TopNodeId = request.TopNodetId,
                NodeType = request.NodeType,
                CreateOn = DateTime.UtcNow,
            };
            ctx.Nodes.Add(item);
            await ctx.SaveChangesAsync();
            return new NodeCommandRespone(true, item);
        }
        catch(Exception ex)
        {
            return new NodeCommandRespone(false, Message: (ex.InnerException is null ? ex.Message : ex.InnerException.Message));
        }
    }
}