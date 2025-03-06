using iPath.Data;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;



public record GetUserResponse(bool Success, string? Message = default!, User Data = null!)
    : BaseResponseT<User>(Success, Message, Data);


public record GetUserQuery(int? Id = null!, string? Username = null!, [EmailAddress] string? Email = null!) : IRequest<GetUserResponse>
{
}


public class GetUserQueryHandler(IDbContextFactory<NewDB> dbFactory) : IRequestHandler<GetUserQuery, GetUserResponse>
{
    public async Task<GetUserResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        User usr = null;
        if (request.Id.HasValue)
        {
            usr = await ctx.Users.FirstOrDefaultAsync(u => u.Id == request.Id);
        }
        else if (!string.IsNullOrWhiteSpace(request.Username))
        {
            usr = await ctx.Users.FirstOrDefaultAsync(u => u.UsernameInvariant == request.Username.ToLowerInvariant());
        }
        else if (!string.IsNullOrWhiteSpace(request.Email))
        {
            usr = await ctx.Users.FirstOrDefaultAsync(u => u.EmailInvariant == request.Email.ToLowerInvariant());
        }

        if (usr is null)
            return new GetUserResponse(false, $"User #{request.Id} not found");
        else
            return new GetUserResponse(true, Data: usr);
    }
}