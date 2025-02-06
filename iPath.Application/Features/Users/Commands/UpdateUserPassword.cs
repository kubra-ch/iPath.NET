using iPath.Data.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public record UpdateUserPasswordCommand(int UserId, string? newPassword, bool IsActive)
    : IRequest<UpdateUserResponse> { }


public class UpdateUserPasswordCommandHandler(IDbContextFactory<IPathDbContext> dbFactory, IPasswordHasher hasher)
    : IRequestHandler<UpdateUserPasswordCommand, UpdateUserResponse>
{
    public async Task<UpdateUserResponse> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // get the User from DB
           using var ctx = await dbFactory.CreateDbContextAsync();
            var item = await ctx.Users.FindAsync(request.UserId);
            if (item == null) return new UpdateUserResponse(false, Message: "user not found");

            if(!string.IsNullOrEmpty(request.newPassword))
            {
                if (request.newPassword.Length < 3)
                    return new UpdateUserResponse(false, Message: "Password must be at least 3 characters long");

                // update properties
                item.PasswordHash = hasher.HashPassword(request.newPassword);
                item.iPath2PasswordHash = null; // delete old ipath 2 hash
            }

            item.IsActive = request.IsActive;
            item.ModifiedOn = DateTime.UtcNow;

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