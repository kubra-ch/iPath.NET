using iPath.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public record DeleteNodeAnnotationCommand(int AnnotationId) : IRequest<BaseResponse>;


public class DeleteNodeAnnotationCommandHandler(IDbContextFactory<NewDB> fct, IMediator mediator)
    : IRequestHandler<DeleteNodeAnnotationCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(DeleteNodeAnnotationCommand request, CancellationToken ctk)
    {
        using var ctx = await fct.CreateDbContextAsync(ctk);
        await ctx.Annotations.Where(a => a.Id == request.AnnotationId).ExecuteDeleteAsync();
        return new SuccessResponse();
    }
}