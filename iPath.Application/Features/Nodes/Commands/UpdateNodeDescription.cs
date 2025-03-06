using iPath.Application.Events;
using iPath.Data;
using iPath.Data.Entities;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public record UpdateNodeDescriptionCommand(int NodeId, NodeDescription Data) : IRequest<BaseResponse>;

public class UpdateNodeDescriptionCommandHandler(IDbContextFactory<NewDB> dbFactory, IMediator mediator) 
    : IRequestHandler<UpdateNodeDescriptionCommand, BaseResponse>
{
    async Task<BaseResponse> IRequestHandler<UpdateNodeDescriptionCommand, BaseResponse>.Handle(UpdateNodeDescriptionCommand request, CancellationToken ctk)
    {
        using var ctx = await dbFactory.CreateDbContextAsync(ctk);
        var node = await ctx.Nodes.FindAsync(request.NodeId, ctk);
        if (node == null)
        {
            return new ErrorResponse("not found");
        }

        node.Description = request.Data;
        node.LastModifiedOn = DateTime.UtcNow;
        ctx.Update(node);
        await ctx.SaveChangesAsync();

        // publish domain event
        await mediator.Publish(new NodeUpdatedEvent(node), ctk);

        return new SuccessResponse();
    }
}