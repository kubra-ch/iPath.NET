using iPath.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;



public record GetCommuniyQuery(int Id) : IRequest<CommunityCommandResponse>;

public class GetCommuniyQueryHandler(IDbContextFactory<NewDB> dbFactory)
    : IRequestHandler<GetCommuniyQuery, CommunityCommandResponse>
{
    public async Task<CommunityCommandResponse> Handle(GetCommuniyQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync(cancellationToken);

        var e = await ctx.Communities
            .Include(c => c.Groups)
            .ThenInclude(cg => cg.Group)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id);

        return new CommunityCommandResponse(true, community: e);
    }
}
