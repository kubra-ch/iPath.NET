using iPath.Application.Events;
using iPath.Data;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public record CreateNodeCommand(Node node) : IRequest<NodeCommandResponse>;

public class CreateNodeCommandHandler(IDbContextFactory<NewDB> fct, IMediator mediator)
    : IRequestHandler<CreateNodeCommand, NodeCommandResponse>
{
    public async Task<NodeCommandResponse> Handle(CreateNodeCommand request, CancellationToken ctk)
    {
        if (request.node.Id != 0) return new NodeCommandResponse(false, "Node must not have an Id != 0");

        using var ctx = await fct.CreateDbContextAsync(ctk);

        await ctx.Nodes.AddAsync(request.node, ctk);
        await ctx.SaveChangesAsync(ctk);

        // dispatch event
        var e = NodeCreatedEvent.CreateNodeEvent(request.node);
        if (e != null)
            await mediator.Publish(e, ctk);

        return new NodeCommandResponse(true, node: request.node);
    }
}