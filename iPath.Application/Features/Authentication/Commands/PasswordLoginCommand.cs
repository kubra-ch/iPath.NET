using iPath.Data;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace iPath.Application.Areas.Authentication.Commands;

public record PasswordLoginCommand(string Username, string Password)
    : IRequest<LoginResponse>;


public class PasswordLoginCommandHandler(IDbContextFactory<NewDB> dbFactory, JwtTokenService srvToken, IPasswordHasher hasher)
    : IRequestHandler<PasswordLoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(PasswordLoginCommand request, CancellationToken cancellationToken)
    {
        // step 1: validate input
        if( string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password) )
        {
            return new LoginResponse(false, Message: "Username and Password must not be empty");
        }

        using var ctx = await dbFactory.CreateDbContextAsync(cancellationToken);

        // step 2: get user and check password
        var uname = request.Username.ToLowerInvariant().Normalize().Trim();
        var user = await ctx.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.UsernameInvariant == uname);

        if ( user is null && uname.Contains("@") )
        {
            user = await ctx.Users.FirstOrDefaultAsync(u => u.EmailInvariant == uname);
        }

        string pwd = hasher.HashPassword(request.Password);                

        if (user == null)
        {
            return new LoginResponse(false, Message: $"User '{request.Username}' not found");
        }

        await MigrateOldPassword(user, request.Password, ctx);

        if( !user.IsActive)
        {
            return new LoginResponse(false, Message: $"User {user.Username} is not active");
        }
        else if (pwd.ToLower() != user.PasswordHash.ToLower())
        {
            return new LoginResponse(false, Message: "Invalid password");
        }
        else
        {
            return await srvToken.GetLoginAsync(user);
        }
    }

    private async Task MigrateOldPassword(User user, string OldPassword, NewDB ctx)
    {
        if(user.PasswordHash == "-" && !string.IsNullOrEmpty(user.iPath2Password))
        {
            var check = CreateMD5(OldPassword).ToLower();
            if (check == user.iPath2Password.ToLower())
            {
                user.PasswordHash = hasher.HashPassword(OldPassword);
                user.iPath2Password = null;
                user.IsActive = true;
                await ctx.SaveChangesAsync();
            }
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
