using iPath.Data;
using iPath.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace iPath.Application.Areas.Authentication;

public class JwtTokenService(IOptions<JwtSettings> opts, IConfiguration config, IDbContextFactory<NewDB> dbFactory)
{
    private JwtSettings settings => opts.Value;

    public string GetToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // basic claims
        var userClaims = new List<Claim>();
        userClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        userClaims.Add(new Claim(ClaimTypes.Name, user.Username));

        // role claims
        if( user.Roles != null )
        {
            foreach (var userRole in user.Roles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, userRole.Name));
            }
        }

        // create token
        var token = new JwtSecurityToken(
          issuer: settings.Issuer,
          audience: settings.Audience,
          claims: userClaims,
          expires: DateTime.UtcNow.AddMinutes(settings.TokenLifetimeInMinutes),
          signingCredentials: credentials
        );

        var ret = new JwtSecurityTokenHandler().WriteToken(token);
        return ret;
    }


    public async Task<LoginResponse> GetLoginAsync(User user)
    {
        // generate the access token
        var token = GetToken(user);

        // generate ans save thre refresh token
        var refresh = await GenerateRefreshTokenAsync(user.Id);

        // create response
        return new LoginResponse(Success: true, AccessToken:token, RefreshToken:refresh);
    }



    public async Task<string> GenerateRefreshTokenAsync(int UserId)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var ti = new UserRefreshToken();
        ti.RefreshToken = Convert.ToBase64String(randomNumber);
        ti.UserId = UserId;
        ti.ExpiredAt = DateTime.UtcNow.AddDays(7);

        // save to db
        using var ctx = dbFactory.CreateDbContext();
        ctx.Set<UserRefreshToken>().Add(ti);
        await ctx.SaveChangesAsync();

        return ti.RefreshToken;
    }

}