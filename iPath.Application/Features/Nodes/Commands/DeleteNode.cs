using iPath.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public record DeleteNodeCommand(int NodeId) : IRequest<BaseResponse>;


public class DeleteNodeCommandHandler(IDbContextFactory<NewDB> fct, IMediator mediator)
    : IRequestHandler<DeleteNodeCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(DeleteNodeCommand request, CancellationToken ctk)
    {
        using var ctx = await fct.CreateDbContextAsync(ctk);
        var node = await ctx.Nodes
            .Include(n => n.ChildNodes)
            .FirstOrDefaultAsync(n => n.Id == request.NodeId);

        if (node is null) return new ErrorResponse("node not found");

        if( node.ChildNodes != null)
        {
            foreach (var child in node.ChildNodes)
            {
                ctx.Nodes.Remove(child);
            }
        }

        ctx.Nodes.Remove(node);
        await ctx.SaveChangesAsync(ctk);
        return new SuccessResponse();
    }
}