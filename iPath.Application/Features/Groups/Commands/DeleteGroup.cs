using iPath.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace iPath.Application.Features;

public record DeleteGroupCommand(int Id) : IRequest<BaseResponse>;


public class DeleteGroupCommandHandler(IDbContextFactory<NewDB> dbFactory, IStringLocalizer T)
    : IRequestHandler<DeleteGroupCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {        
        using var ctx = await dbFactory.CreateDbContextAsync(cancellationToken);

        var e = await ctx.Groups.FindAsync(request.Id);
        if (e is null) return new GroupCommandResponse(false, T["Group {0} not found", request.Id]);

        ctx.Groups.Remove(e);
        await ctx.SaveChangesAsync(cancellationToken);
        return new SuccessResponse();
    }
}