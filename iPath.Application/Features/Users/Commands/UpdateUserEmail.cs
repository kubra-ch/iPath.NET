using iPath.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public record UpdateUserEmailCommand(int UserId, [EmailAddress] string Email)
    : IRequest<UserCommandResponse>
{ }


public class UpdateUserEmailCommandHandler(IDbContextFactory<NewDB> dbFactory)
    : IRequestHandler<UpdateUserEmailCommand, UserCommandResponse>
{
    public async Task<UserCommandResponse> Handle(UpdateUserEmailCommand request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();

        // trim
        var email = request.Email.Trim();

        // find other user with different id but same new name
        var exists = await ctx.Users.AnyAsync(u => u.EmailInvariant == email.ToLowerInvariant() && u.Id != request.UserId);
        if (exists)
            return new UserCommandResponse(false, Message: $"Another User with email {email} already exists");


        // get the User from DB
        var item = await ctx.Users.FindAsync(request.UserId);

        // update properties
        item.Email = email;
        item.EmailInvariant = email.ToLowerInvariant();
        item.ModifiedOn = DateTime.UtcNow;

        ctx.Users.Update(item);
        await ctx.SaveChangesAsync();
        return new UserCommandResponse(true, Data: item);
    }
}