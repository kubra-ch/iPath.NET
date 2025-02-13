using iPath.Application.Querying;
using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace iPath.Application.Features;




public record GetGroupListDtoResponse(bool Success, string? Message = default!, PaginatedListResult<GroupListDto> Data = null!)
    : BaseResponseT<PaginatedListResult<GroupListDto>>(Success, Message, Data);


public class GetGroupListDtoQuery : PaginatedListQuery, IRequest<GetGroupListDtoResponse>
{
    public int? UserId { get; set; }
    public int? CommunityId { get; set; }
}


public class GetGroupListDtoQueryHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<GetGroupListDtoQuery, GetGroupListDtoResponse>
{
    public async Task<GetGroupListDtoResponse> Handle(GetGroupListDtoQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        var q = ctx.Groups
            .Include(g => g.Community)
            .AsNoTracking()
            .AsQueryable();

        // filter by user
        if( request.UserId.HasValue)
        {
            // personal list
            q = q.Where(g => g.Members.Any(m => m.UserId == request.UserId));
        }
        else
        {
            // list of public groups
            q = q.Where(g => g.Visibility == eGroupVisibility.Public);
        }
           
        // filter by community
        if( request.CommunityId.HasValue)
        {
            q = q.Where(g => g.CommunityId == request.CommunityId.Value);
        }

        // get the total group count
        var total = await q.AsNoTracking().CountAsync();

        // sorting 
        // q = q.ApplySort(request.SortDefinitions);
        var sd = request.SortDefinitions.FirstOrDefault();
        if (sd != null)
        {
            if(sd.SortAscending)
            {
                q = q.OrderBy(g => g.Name);
            }
            else
            {
                q = q.OrderByDescending(g => g.Name);
            }
        }

        // pagination
        if (request.Count.HasValue)
            q = q.Skip(request.StartIndex * request.Count.Value).Take(request.Count.Value);

        // get paged data with object count, etx
        var dtos = await q.Include(g => g.Nodes)
            .AsNoTracking()
            .Select(g => new GroupListDto(g.Id, g.Name, g.CommunityId, g.Nodes.Count(), 0, 0))
            .ToListAsync();

        var ret = new PaginatedListResult<GroupListDto>()
        {
            TotalItemsCount = total,
            Items = dtos
        };

        return new GetGroupListDtoResponse(true, Data: ret);
    }
}


