using iPath.Application.Querying;
using iPath.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace iPath.Application.Features;


public record GetNodeIdListQuery(int? GroupId, int? UserId, string? SearchString, List<SortDefinition> sorting = null!) : IRequest<BaseResponseT<List<int>>>;


public class GetNodeIdListQueryHandler(IDbContextFactory<NewDB> dbFactory)
    : IRequestHandler<GetNodeIdListQuery, BaseResponseT<List<int>>>
{
    public async Task<BaseResponseT<List<int>>> Handle(GetNodeIdListQuery request, CancellationToken ctk)
    {
        // check that user is int group
        /*
        if( request.GroupId.HasValue )
        {
            var check = await authManager.IsMember(request.GroupId.Value);
            if (!check) return new BaseResponseT<List<int>>(false, Message: "no access");
        }
        */

        using var ctx = await dbFactory.CreateDbContextAsync(ctk);

        var q = ctx.PrepareNodeQuery(UserId: request.UserId, GroupId: request.GroupId, SearchString: request.SearchString);
        if (request.sorting != null) q = q.ApplySort(request.sorting);

        var ids = await q.Select(n => n.Id).ToListAsync();

        return new BaseResponseT<List<int>>(true, Data: ids);
    }
}