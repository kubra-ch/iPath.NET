using iPath.Application.Authentication;
using iPath.Application.Querying;
using iPath.Data;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

/*
public record GroupListResponse(bool Success, string? Message = default!, GroupDTO[]? Data = null!)
    : BaseResponseT<GroupDTO[]>(Success, Message, Data);
*/ 

public class GetGroupListQuery : PaginatedListQuery, IRequest<PaginatedListResult<GroupDTO>>
{
    public int? UserId { get; set; } 
    public int? ModeratorId { get; set; } 
    public int? CommunityId { get; set; }
    public bool IncludeNodeCount { get; set; }
}


// public record GetGroupsListQuery(int? UserId=null!, int? ModeratorId=null!, int? CommunityId = null!) : IRequest<PaginatedListResult<GroupDTO>>;


public class GetAllGroupsListQueryHandler(IDbContextFactory<NewDB> fct) : IRequestHandler<GetGroupListQuery, PaginatedListResult<GroupDTO>>
{
    public async Task<PaginatedListResult<GroupDTO>> Handle(GetGroupListQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await fct.CreateDbContextAsync();
        var q = ctx.Groups.AsNoTracking().AsQueryable();

        if(request.UserId.HasValue)
        {
            q = q.Where(g => g.Members.Any(m => m.UserId == request.UserId));
        }

        if(request.ModeratorId.HasValue)
        {
            q = q.Where(g => g.Members.Any(m => m.UserId == request.ModeratorId && m.Role.HasFlag(eMemberRole.Moderator)));
        }

        if(request.CommunityId.HasValue)
        {
            q = q.Where(g => g.Communities.Any(c => c.CommunityId == request.CommunityId.Value));
        }

        var nameSearch = request.GetFilterValue("name");
        if (!string.IsNullOrWhiteSpace(nameSearch))
        {
            q = q.Where(c => EF.Functions.ILike(c.Name, $"%{nameSearch}%"));
        }

        var total = await q.CountAsync();

        q = q.ApplySort(request.SortDefinitions);
        q = q.ApplyPagination(request);



        GroupDTO[] dto;
        if (request.IncludeNodeCount)
        {
            dto = await q.Select(g => new GroupDTO { Id = g.Id, Name = g.Name, NodeCount = g.Nodes.Count() }).ToArrayAsync();
        }
        else
        {
            dto = await q.Select(g => new GroupDTO { Id = g.Id, Name = g.Name }).ToArrayAsync();
        }

        return new PaginatedListResult<GroupDTO>(TotalItemsCount: total, Data: dto);
    }
}
