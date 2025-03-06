using iPath.Application.Events;
using iPath.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public record CreateNodeAnnotationCommand(int NodeId, int UserId, string Text) : IRequest<BaseResponse>;


public class CreateNodeAnnotationCommandHandler(IDbContextFactory<NewDB> fct, IMediator mediator)
    : IRequestHandler<CreateNodeAnnotationCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(CreateNodeAnnotationCommand request, CancellationToken ctk)
    {
        if (string.IsNullOrEmpty(request.Text)) return new ErrorResponse("text is empty");

        using var ctx = await fct.CreateDbContextAsync(ctk);
        var node = await ctx.Nodes.FindAsync(request.NodeId);
        if (node is null) return new ErrorResponse("node not found");

        var owner = await ctx.Users.FindAsync(request.UserId, ctk);
        if (owner is null) return new ErrorResponse("user not found");

        var a = new Data.Entities.Annotation
        {
            Text = request.Text,
            OwnerId = request.UserId,
            CreatedOn = DateTime.UtcNow,
        };
        node.Annotations.Add(a);

        ctx.Nodes.Update(node);
        await ctx.SaveChangesAsync(ctk);

        // publish domain event
        await mediator.Publish(new AnnotationAddedEvent(a, node), ctk);
        await mediator.Publish(new NodeUpdatedEvent(node), ctk);

        return new SuccessResponse();
    }
}