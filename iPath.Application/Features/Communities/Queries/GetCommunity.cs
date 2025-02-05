using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class GetCommunityQuery : IRequest<Community>
{
    public int Id { get; set; }
}

public class GetCommunityQueryHandler(IPathDbContext ctx) : IRequestHandler<GetCommunityQuery, Community>
{
    public async Task<Community> Handle(GetCommunityQuery request, CancellationToken cancellationToken)
    {
        return await ctx.Communities 
            .Include(c => c.Groups)
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(u => u.Id == request.Id);
    }
}