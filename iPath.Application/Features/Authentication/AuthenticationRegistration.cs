using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace iPath.Application.Areas.Authentication;

public static class AuthenticationRegistration
{
    public static IServiceCollection AddIPathAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Load JWT Configurations
        var mJwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

        // base services
        services.AddTransient<IPasswordHasher, PasswordHasher>();
        services.AddTransient<JwtTokenService>();


        // Configure JWT
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = mJwtSettings?.Issuer,
                    ValidAudience = mJwtSettings?.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(mJwtSettings?.Key))
                };
            });

        // Add policies for authentication schemes
        services.AddAuthorization(options =>
        {
            // Default policy should allow for jwt
            var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
            options.DefaultPolicy = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser().Build();
        });


        return services;
    }



    public static WebApplication MapAuthenticationEndpoint(this WebApplication app)
    {
        app.MapPost("/api/auth/login", async (LoginRequest request, [FromServices] IMediator mediador) =>
        {
            return await mediador.Send(request);
        });

        return app;
    }
}
