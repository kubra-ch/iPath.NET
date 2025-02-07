using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace iPath.Application.Features;

public record LoginResponse(bool Success, string? Message = default!, User? Data = null!) :
    BaseResponseT<User>(Success, Message, Data);


public record LoginRequest(string Password, string? Username = default!, string? Email = default!) : IRequest<LoginResponse>;


public class LoginRequestHandler(IDbContextFactory<IPathDbContext> dbFactory, IPasswordHasher hasher) : IRequestHandler<LoginRequest, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
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



        // convert from old system?
        if (!user.IsActive && !string.IsNullOrEmpty(user.iPath2PasswordHash))
        {
            var check = CreateMD5(request.Password).ToLower();
            if (check == user.iPath2PasswordHash.ToLower())
            {
                user.PasswordHash = hasher.HashPassword(request.Password);
                user.IsActive = true;
                user.EmailInvariant = user.Email.Trim().ToLowerInvariant();
                user.UsernameInvariant = user.Username.Trim().ToLowerInvariant();
                await ctx.SaveChangesAsync();
            }
        }

        if (!hasher.VerifyHashedPassword(user.PasswordHash, request.Password))
        {
            return new LoginResponse(false, Message: "Invalid password");
        }

        return new LoginResponse(true, Data: user);
    }



    private static string CreateMD5_1(string input)
    {
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes);
        }
    }

    private static string CreateMD5(string input)
    {
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
