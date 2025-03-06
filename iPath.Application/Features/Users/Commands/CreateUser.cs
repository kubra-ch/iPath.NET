using iPath.Application.Areas.Authentication;
using iPath.Data;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public record CreateUserCommand(string Username, string Email, string Password) : IRequest<UserCommandResponse>
{
}


public class CreateUserCommandHandler(IDbContextFactory<NewDB> dbFactory, IPasswordHasher hasher)
    : IRequestHandler<CreateUserCommand, UserCommandResponse>
{
    public async Task<UserCommandResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        // check that neither username or email is used
        if (await ctx.Users.AnyAsync(u => u.UsernameInvariant == request.Username.ToLowerInvariant()))
        {
            return new UserCommandResponse(false, Message: "Username already in use");
        }
        else if (await ctx.Users.AnyAsync(u => u.EmailInvariant == request.Email.ToLowerInvariant()))
        {
            return new UserCommandResponse(false, Message: "Email already in use");
        }

        User usr = new User
        {
            Username = request.Username,
            UsernameInvariant = request.Username.ToLowerInvariant(),
            Email = request.Email,
            EmailInvariant = request.Email.ToLowerInvariant(),
            IsActive = true,
            PasswordHash = hasher.HashPassword(request.Password),
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow
        };

        ctx.Users.Add(usr);
        await ctx.SaveChangesAsync();
        return new UserCommandResponse(true, Data: usr);
    }
}