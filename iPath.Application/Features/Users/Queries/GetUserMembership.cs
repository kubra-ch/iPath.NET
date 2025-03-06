using iPath.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;


public record GetUserMembershipQuery(int UserId) : IRequest<GetUserResponse>;


public class GetUserMembershipQueryHandler(IDbContextFactory<NewDB> fct) : IRequestHandler<GetUserMembershipQuery, GetUserResponse>
{
    public async Task<GetUserResponse> Handle(GetUserMembershipQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await fct.CreateDbContextAsync();
        var usr = await ctx.Users.AsNoTracking()
            .Include(u => u.GroupMembership)
            .FirstOrDefaultAsync(u => u.Id == request.UserId);
        return new GetUserResponse(usr != null, Data: usr, Message: usr == null ? "user not found" : null);
    }
}
