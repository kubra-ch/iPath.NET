using iPath.Application.Querying;
using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class GetUserListQuery : PaginatedListQuery, IRequest<PaginatedListResult<User>>
{
}


public class GetUserListQueryHandler(IPathDbContext ctx)
    : IRequestHandler<GetUserListQuery, PaginatedListResult<User>>
{
    public async Task<PaginatedListResult<User>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
    {
        var q = ctx.Users.AsNoTracking().AsQueryable();
        if( request.Filter != null)
        {
           foreach (var f in request.Filter.Filters)
            {
                if (!string.IsNullOrEmpty(f.Field) && f.Value != null)
                {
                    switch (f.Field.ToLowerInvariant())
                    {
                        case "username":
                            q = q.Where(u => u.UsernameInvariant.Contains(f.Value.ToString()));
                            break;
                        case "email":
                            q = q.Where(u => u.EmailInvariant.Contains(f.Value.ToString()));
                            break;
                    }
                }
            }
        }

        return await q.GetPaginatedListResultAsync(request);
    }
}

