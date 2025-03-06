using iPath.Data;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;



public record GetUserListResponse(bool Success, string? Message = default!, List<UserDTO> Data = null!)
    : BaseResponseT<List<UserDTO>>(Success, Message, Data);


public record GetUserListQuery(string name) : IRequest<GetUserListResponse>;


public class GetUserListQueryHandler(IDbContextFactory<NewDB> dbFactory) : IRequestHandler<GetUserListQuery, GetUserListResponse>
{
    public async Task<GetUserListResponse> Handle(GetUserListQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();

        var list = await ctx.Users.AsNoTracking()
            .Where(u => EF.Functions.ILike(u.Username, $"{request.name}%"))
            .OrderBy(u => u.Username)
            .Take(100)
            .Select(u => new UserDTO { UserId = u.Id, Username = u.Username, Initials = u.Profile.Initials})
            .ToListAsync();

        return new GetUserListResponse(true, Data: list);
    }
}