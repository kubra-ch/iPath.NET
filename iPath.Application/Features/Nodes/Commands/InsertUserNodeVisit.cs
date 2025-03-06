using iPath.Data;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;


public record InsertUserNodeVisitCommand(int UserId, int NodeId) : IRequest<BaseResponse>;

public class InsertUserNodeVisitCommandHandler(IDbContextFactory<NewDB> dbFactory)
    : IRequestHandler<InsertUserNodeVisitCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(InsertUserNodeVisitCommand request, CancellationToken ctk)
    {
        using var ctx = await dbFactory.CreateDbContextAsync(ctk);
        var v = await ctx.Set<NodeLastVisit>().FirstOrDefaultAsync(v => v.UserId == request.UserId && v.NodeId == request.NodeId);
        if (v != null)
        {
            v.Date = DateTime.UtcNow;
            ctx.Set<NodeLastVisit>().Update(v);
        }
        else
        {
            v = new NodeLastVisit { UserId = request.UserId, NodeId = request.NodeId, Date = DateTime.UtcNow };
            ctx.Set<NodeLastVisit>().Add(v);
        }
        await ctx.SaveChangesAsync();
        return new SuccessResponse();
    }
}
