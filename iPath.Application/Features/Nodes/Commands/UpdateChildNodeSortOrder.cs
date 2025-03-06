using iPath.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public record UpdateChildNodeSortOrderCommand(int NodeId, Dictionary<int, int> sortOrder) : IRequest<BaseResponse>;

public class UpdateChildNodeSortOrderCommandHandler(IDbContextFactory<NewDB> dbFactory) 
    : IRequestHandler<UpdateChildNodeSortOrderCommand, BaseResponse>
{
    async Task<BaseResponse> IRequestHandler<UpdateChildNodeSortOrderCommand, BaseResponse>.Handle(UpdateChildNodeSortOrderCommand request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        var nodes = await ctx.Nodes.Where(n => request.sortOrder.Keys.Contains(n.Id)).ToListAsync();

        foreach( var child in nodes )
        {
            child.SortNr = request.sortOrder[child.Id];
            child.LastModifiedOn = DateTime.UtcNow;
            ctx.Update(child);
        }

        await ctx.SaveChangesAsync();
        return new SuccessResponse();
    }
}