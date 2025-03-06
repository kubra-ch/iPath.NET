using iPath.Application.Querying;
using iPath.Data;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class GetCommuniyListQuery() : PaginatedListQuery, IRequest<PaginatedListResult<Community>>
{
    public int? UserId { get; set; }
}

public class GetCommuniyListQueryHandler(IDbContextFactory<NewDB> dbFactory)
    : IRequestHandler<GetCommuniyListQuery, PaginatedListResult<Community>>
{
    public async Task<PaginatedListResult<Community>> Handle(GetCommuniyListQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync(cancellationToken);

        var q = ctx.Communities.AsNoTracking().AsQueryable();

        var nameSearch = request.GetFilterValue("name");
        if (!string.IsNullOrWhiteSpace(nameSearch))
        {
            q = q.Where(c => EF.Functions.ILike(c.Name, $"%{nameSearch}%"));
        }
        if(request.UserId.HasValue)
        {
            q = q.Where(q => q.Members.Any(m => m.UserId == request.UserId.Value));
        }

        return await q.GetPaginatedListResultAsync(request);
    }
}
