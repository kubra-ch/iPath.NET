using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public record CreateUserCommand(string Username, string Email, string Password) : IRequest<CreateUserResponse>
{
}


public record CreateUserResponse(bool Success, User? User = null!, string? Message = default!);



public class CreateUserCommandHandler(IDbContextFactory<IPathDbContext> dbFactory, IPasswordHasher hasher)
    : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        // check that neither username or email is used
        if ( await ctx.Users.AnyAsync (u => u.UsernameInvariant == request.Username.ToLowerInvariant()))
        {
            return new CreateUserResponse(false, Message: "Username already in use");
        }
        else if (await ctx.Users.AnyAsync(u => u.EmailInvariant == request.Email.ToLowerInvariant()))
        {
            return new CreateUserResponse(false, Message: "Email already in use");
        }

        try
        {
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
            return new CreateUserResponse(true, usr);
        }
        catch(Exception ex)
        {
            return new CreateUserResponse(false, Message: ex.Message);
        }
    }
}