using iPath.Application.Querying;
using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;




public record GetUserListDtoResponse(bool Success, string? Message = default!, PaginatedListResult<UserListDto> Data = null!)
    : BaseResponseT<PaginatedListResult<UserListDto>>(Success, Message, Data);


public class GetUserListDtoQuery : PaginatedListQuery, IRequest<GetUserListDtoResponse>
{
    public bool IsActive { get; set; }
}


public class GetUserListDtoQueryHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<GetUserListDtoQuery, GetUserListDtoResponse>
{
    public async Task<GetUserListDtoResponse> Handle(GetUserListDtoQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        var q = ctx.Users.AsNoTracking().AsQueryable();

        if( request.IsActive)
        {
            q = q.Where(x => x.IsActive);
        }

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

        var data = await q.Select(u => new UserListDto(u.Id, u.Username, u.Email)).GetPaginatedListResultAsync(request);
        return new GetUserListDtoResponse(true, Data: data);
    }
}

