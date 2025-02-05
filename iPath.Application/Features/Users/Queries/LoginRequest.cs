using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public record LoginRequest(string Password, string? Username = default!,string? Email = default!) : IRequest<LoginResponse>;

public record LoginResponse(bool Success, User? User = null!, string? Message = default!);

public class LoginRequestHandler(IPathDbContext ctx, IPasswordHasher hascher) : IRequestHandler<LoginRequest, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            User user = null!;
            if (!string.IsNullOrWhiteSpace(request.Username))
            {
                user = await ctx.Users.FirstOrDefaultAsync(u => u.UsernameInvariant == request.Username.ToLowerInvariant());
            }
            else if (!string.IsNullOrWhiteSpace(request.Email))
            {
                user = await ctx.Users.FirstOrDefaultAsync(u => u.EmailInvariant == request.Email.ToLowerInvariant());
            }

            if (user is null)
            {
                return new LoginResponse(false, Message: "User not found");
            }
            else if (!hascher.VerifyHashedPassword(user.PasswordHash, request.Password))
            {
                return new LoginResponse(false, Message: "Invalid password");
            }

            return new LoginResponse(true, user);
        }
        catch (Exception ex)
        {
            return new LoginResponse(false, Message: ex.Message);
        }
    }
}
