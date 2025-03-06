using iPath.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;


public record UpdateUsernameCommand(int UserId, string Username)
    : IRequest<UserCommandResponse>
{ }


public class UpdateUserNameCommandHandler(IDbContextFactory<NewDB> dbFactory)
    : IRequestHandler<UpdateUsernameCommand, UserCommandResponse>
{
    public async Task<UserCommandResponse> Handle(UpdateUsernameCommand request, CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();

        if (username.Length < 3)
            return new UserCommandResponse(false, Message: "Username must be at least 3 characters long");

        // find other user with different id but same new name
        using var ctx = await dbFactory.CreateDbContextAsync();
        var exists = await ctx.Users.AnyAsync(u => u.UsernameInvariant == username.ToLowerInvariant() && u.Id != request.UserId);
        if (exists)
            return new UserCommandResponse(false, Message: $"Another User with name {username} already exists");


        // get the User from DB
        var item = await ctx.Users.FindAsync(request.UserId);

        // update properties
        item.Username = username;
        item.UsernameInvariant = username.ToLowerInvariant();
        item.ModifiedOn = DateTime.UtcNow;

        ctx.Users.Update(item);
        await ctx.SaveChangesAsync();
        return new UserCommandResponse(true, Data: item);
    }
}