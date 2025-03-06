using MudBlazor.Services;
using Blazored.LocalStorage;
using iPath.UI.Components;
using iPath.Data.EFCore;
using iPath.Application;
using iPath.UI.Areas.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.OpenApi.Models;
using iPath.UI.Areas.DataAccess;
using iPath.UI.Components.Nodes.ViewModels;
using iPath.UI.Components.Users.ViewModels;
using iPath.Application.Authentication;
using iPath.API;
using iPath2.DataImport;
using iPath.UI.Components.Admin;
using iPath.UI.Components.Communities;
using iPath.UI.Components.Groups;
using iPath.UI.Areas.BreadCrumbs;

namespace iPath.UI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add MudBlazor services
        builder.Services.AddMudServices();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // SeriLog
        /*
        builder.Host.UseSerilog();
        Log.Logger = new LoggerConfiguration()
          .ReadFrom.Configuration(builder.Configuration)
          .CreateLogger();
        */

        // Backend Services
        builder.Services.AddPersistance(builder.Configuration);
        builder.Services.AddIPath2DataImport(builder.Configuration);
        builder.Services.AddApplicationServices(builder.Configuration);
        builder.Services.AddApiServices(builder.Configuration);

        // http client
        builder.Services.AddTransient<JwtAuthorizationMessageHandler>();
        var apiUrl = builder.Configuration["ApiSettings:Url"];
        builder.Services.AddHttpClient("Api", client => client.BaseAddress = new Uri(apiUrl))
            .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();


        // controllers
        builder.Services.AddControllers();      

        // UI Services
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddSingleton<TokenCache>();
        builder.Services.AddScoped<ITokenStore, LocalTokenStore>();
        builder.Services.AddScoped<IAuthService, AuthServiceMediator>();
        builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

        builder.Services.AddScoped<IDataAccess, DataAccessMediator>();
        builder.Services.AddScoped<IAuthManager, AuthManager>();
        builder.Services.AddScoped<AuthManagerBlazor>();
        builder.Services.AddScoped<BreadCrumbService>();

        // View Models
        builder.Services.AddScoped<NodeListViewModel>();
        builder.Services.AddScoped<NodeDetailViewModel>();
        builder.Services.AddScoped<UserProfileViewModel>();
        builder.Services.AddScoped<CommunityAdminViewModel>();
        builder.Services.AddScoped<GroupAdminViewModel>();
        builder.Services.AddScoped<UserAdminViewModel>();
        builder.Services.AddScoped<GroupListViewModel>();



        // Setup CORS (cross origin resource sharing)
        builder.Services.AddCors(
            opts => opts.AddPolicy(
                "open",
                policy => policy.WithOrigins([
                    builder.Configuration["ApiSettings:Url"],
                    builder.Configuration["ApiSettings:Url"]
                ])
                .AllowAnyMethod()
                .SetIsOriginAllowed(pol => true)
                .AllowAnyHeader()
                .AllowCredentials()
            ));





        // Swagger
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "iPath API",
                Version = "v1",
                Description = "API to access iPath.NET"
            });
        });
        builder.Services.AddEndpointsApiExplorer();



        // build the web app
        var app = builder.Build();

        // Run Db Seeder
        app.SeedDatase();

        // Apply CORS
        app.UseCors("open");

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        else
        {
            app.UseSwagger();
            app.UseSwaggerUI(opts =>
            {
                opts.EnableTryItOutByDefault();
            });
        }
        
        
        app.UseHttpsRedirection();

        app.UseAntiforgery();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHubs();
        app.MapStaticAssets();

        app.UseStatusCodePagesWithRedirects("/404");

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();


        app.Run();
    }
}
