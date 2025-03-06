using iPath.Data;
using iPath.Application.Querying;
using MediatR;
using Microsoft.EntityFrameworkCore;
using iPath.Data.Entities;
using System.Linq.Dynamic.Core;

namespace iPath.Application.Features;


public class GetNodeListQuery : PaginatedListQuery, IRequest<NodeListResponse>
{
    public int? GroupId { get; set; }
    public int? UserId { get; set; }
    public string SearchString { get; set; }
}


public class GetNodesListQueryHandler(IDbContextFactory<NewDB> dbFactory)
    : IRequestHandler<GetNodeListQuery, NodeListResponse>
{
    public async Task<NodeListResponse> Handle(GetNodeListQuery request, CancellationToken ctk)
    {
        // check that user is int group
        /*
        if( request.GroupId.HasValue )
        {
            var check = await authManager.IsMember(request.GroupId.Value);
            if (!check) return new NodeListResponse(false, Message: "you are not member of group " + request.GroupId);
        }
        */

        using var ctx = await dbFactory.CreateDbContextAsync(ctk);

        var q = ctx.PrepareNodeQuery(UserId: request.UserId, GroupId: request.GroupId, SearchString: request.SearchString); 

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