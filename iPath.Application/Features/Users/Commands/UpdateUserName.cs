using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public record UpdateUserNameCommand(int UserId, string Username)
    : IRequest<UpdateUserResponse> { }


public class UpdateUserNameCommandHandler(IPathDbContext ctx)
    : IRequestHandler<UpdateUserNameCommand, UpdateUserResponse>
{
    public async Task<UpdateUserResponse> Handle(UpdateUserNameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var username = request.Username.Trim();

            if(username.Length < 3)
                return new UpdateUserResponse(false, Message: "Username must be at least 3 characters long");

            // find other user with different id but same new name
            var exists = await ctx.Users.AnyAsync(u => u.UsernameInvariant == username.ToLowerInvariant() && u.Id != request.UserId);
            if( exists)
                return new UpdateUserResponse(false, Message: $"Another User with name {username} already exists"); 


            // get the User from DB
            var item = await ctx.Users.FindAsync(request.UserId);

            // update properties
            item.Username = username;
            item.UsernameInvariant = username.ToLowerInvariant();

            ctx.Users.Update(item);
            await ctx.SaveChangesAsync();
            return new UpdateUserResponse(true, item);
        }
        catch(Exception ex)
        {
            return new UpdateUserResponse(false, Message: (ex.InnerException is null ? ex.Message : ex.InnerException.Message));
        }
    }
}