using iPath.Application.Areas.Authentication;
using iPath.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public record UpdateUserPasswordCommand(int UserId, string? newPassword, bool IsActive)
    : IRequest<UserCommandResponse>
{ }


public class UpdateUserPasswordCommandHandler(IDbContextFactory<NewDB> dbFactory, IPasswordHasher hasher)
    : IRequestHandler<UpdateUserPasswordCommand, UserCommandResponse>
{
    public async Task<UserCommandResponse> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        // get the User from DB
        using var ctx = await dbFactory.CreateDbContextAsync();
        var item = await ctx.Users.FindAsync(request.UserId);
        if (item == null) return new UserCommandResponse(false, Message: "user not found");

        if (!string.IsNullOrEmpty(request.newPassword))
        {
            if (request.newPassword.Length < 3)
                return new UserCommandResponse(false, Message: "Password must be at least 3 characters long");

            // update properties
            item.PasswordHash = hasher.HashPassword(request.newPassword);
            item.iPath2Password = null; // delete old ipath 2 hash
        }

        item.IsActive = request.IsActive;
        item.ModifiedOn = DateTime.UtcNow;

        ctx.Users.Update(item);
        await ctx.SaveChangesAsync();
        return new UserCommandResponse(true, Data: item);
    }
}