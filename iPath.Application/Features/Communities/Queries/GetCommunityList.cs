﻿using iPath.Application.Querying;
using iPath.Data.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class GetCommunityListQuery : PaginatedListQuery, IRequest<GetCommunityListResponse>
{
}


public record GetCommunityListResponse(bool Success, string? Message = default!, PaginatedListResult<CommunityDto> Data = null!)
    : BaseResponseT<PaginatedListResult<CommunityDto>>(Success, Message, Data);



public class GetCommunityListQueryHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<GetCommunityListQuery, GetCommunityListResponse>
{
    public async Task<GetCommunityListResponse> Handle(GetCommunityListQuery request, CancellationToken cancellationToken)
    {
       using var ctx = await dbFactory.CreateDbContextAsync();
        var q = ctx.Communities.AsNoTracking().AsQueryable();

        if( request.Filter != null)
        {
           foreach (var f in request.Filter.Filters)
            {
                if (!string.IsNullOrEmpty(f.Field) && f.Value != null)
                {
                    switch (f.Field.ToLowerInvariant())
                    {
                        case "name":
                            q = q.Where(u => u.Name.Contains(f.Value.ToString()));
                            break;
                    }
                }
            }
        }

        var entities = await q.GetPaginatedListResultAsync(request);

        var data = new PaginatedListResult<CommunityDto>(entities.Items.Select(x => x.ToDto()).ToList(), entities.TotalItemsCount);

        return new GetCommunityListResponse(true, Data: data);
    }
}

