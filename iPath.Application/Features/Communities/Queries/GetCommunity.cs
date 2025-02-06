using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class GetCommunityQuery : IRequest<Community>
{
    public int Id { get; set; }
}

public class GetCommunityQueryHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<GetCommunityQuery, Community>
{
    public async Task<Community> Handle(GetCommunityQuery request, CancellationToken cancellationToken)
    {
       using var ctx = await dbFactory.CreateDbContextAsync();
        return await ctx.Communities 
            .Include(c => c.Groups)
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.Id);
    }
}