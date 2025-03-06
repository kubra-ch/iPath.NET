using iPath.Application.Querying;
using iPath.Data;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class GetRecentlyVisitedNodesQuery : PaginatedListQuery, IRequest<NodeListResponse>
{
    public int? UserId { get; set; }
}

public class GetRecentlyVisitedNodesQueryHandler(IDbContextFactory<NewDB> dbFactory)
    : IRequestHandler<GetRecentlyVisitedNodesQuery, NodeListResponse>
{
    public async Task<NodeListResponse> Handle(GetRecentlyVisitedNodesQuery request, CancellationToken ctk)
    {

        using var ctx = await dbFactory.CreateDbContextAsync(ctk);

        var q = ctx.PrepareNodeQuery(UserId: request.UserId, RecentlyVisited: true);
        var totalItems = await q.CountAsync();
        q = q.ApplySort(request.SortDefinitions);
        q = q.ApplyPagination(request);


        var items = await q.Select(n => new NodeListDTO
        {
            Id = n.Id,
            CreatedOn = n.CreatedOn,
            Title = n.Description.Title,
            SubTitle = n.Description.Subtitle,
            CaseType = n.Description.CaseType,
            AccessionNo = n.Description.AccessionNo,
            Owner = new UserDTO { UserId = n.OwnerId, Username = n.Owner.Username }
        })
            .ToArrayAsync();

        return new NodeListResponse(Data: items, TotalItemsCount: totalItems);
    }
}
