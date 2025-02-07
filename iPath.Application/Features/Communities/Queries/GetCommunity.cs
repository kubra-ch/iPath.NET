using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public record GetCommunityQuery(int Id) : IRequest<GetCommunityResponse>
{
}


public record GetCommunityResponse(bool Success, string? Message = default!, Community Data = null!)
    : BaseResponseT<Community>(Success, Message, Data);



public class GetCommunityQueryHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<GetCommunityQuery, GetCommunityResponse>
{
    public async Task<GetCommunityResponse> Handle(GetCommunityQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        var data = await ctx.Communities 
            .Include(c => c.Groups)
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.Id);

        if (data == null)
            return new GetCommunityResponse(false, Message: $"Community #{request.Id} not found");
        else
            return new GetCommunityResponse(true, Data: data);
    }
}