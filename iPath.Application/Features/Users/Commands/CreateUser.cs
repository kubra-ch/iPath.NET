using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public class CreateUserCommand : IRequest<CreateUserResponse>
{
    [MinLength(3)] 
    public string Username { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [MinLength(4)]
    public string Password { get; set; }
}


public record CreateUserResponse(bool Success, User? User = null!, string? Message = default!);



public class CreateUserCommandHandler(IPathDbContext ctx, IPasswordHasher hasher)
    : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // check that neither username or email is used
        if( await ctx.Users.AnyAsync (u => u.UsernameInvariant == request.Username.ToLowerInvariant()))
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
                PasswordHash = hasher.HashPassword(request.Password)
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