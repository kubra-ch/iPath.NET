using iPath.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;



public record GetGroupQuery(int Id) : IRequest<GroupCommandResponse>;

public class GetGroupQueryHandler(IDbContextFactory<NewDB> dbFactory)
    : IRequestHandler<GetGroupQuery, GroupCommandResponse>
{
    public async Task<GroupCommandResponse> Handle(GetGroupQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync(cancellationToken);

        var e = await ctx.Groups
            .Include(c => c.Communities)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id);

        return new GroupCommandResponse(true, group: e);
    }
}
