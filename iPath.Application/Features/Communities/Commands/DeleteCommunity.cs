using iPath.Data;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public record DeleteCommunityCommand(int Id) : IRequest<BaseResponse>;


public class DeleteCommunityCommandHandler(IDbContextFactory<NewDB> dbFactory, IStringLocalizer T)
    : IRequestHandler<DeleteCommunityCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(DeleteCommunityCommand request, CancellationToken cancellationToken)
    {        
        using var ctx = await dbFactory.CreateDbContextAsync(cancellationToken);

        var e = await ctx.Communities.FindAsync(request.Id);
        if (e is null) return new GroupCommandResponse(false, T["Community {0} not found", request.Id]);

        ctx.Communities.Remove(e);
        await ctx.SaveChangesAsync(cancellationToken);
        return new SuccessResponse();
    }
}