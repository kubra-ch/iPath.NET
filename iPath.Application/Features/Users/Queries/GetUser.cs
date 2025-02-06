using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public class GetUserQuery : IRequest<User>
{
    public int? Id { get; set; }
    public string? Username { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
}

public class GetUserQueryHandler(IDbContextFactory<IPathDbContext> dbFactory) : IRequestHandler<GetUserQuery, User>
{
    public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        User usr = null;
        if(request.Id.HasValue)
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
        return usr;
    }
}