using iPath.UI;
using iPath.UI.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using iPath.Application;
using iPath.Application.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using iPath.UI.Areas.Identity;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Serilog;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddFluentUIComponents();
        builder.Services.AddHttpContextAccessor();

        // configuration
        builder.Services.Configure<iPathConfig>(builder.Configuration.GetSection(nameof(iPathConfig)));
        var config = new iPathConfig();
        builder.Configuration.GetSection(nameof(iPathConfig)).Bind(config);

        // SeriLog
        builder.Host.UseSerilog();
        Log.Logger = new LoggerConfiguration()
          .ReadFrom.Configuration(builder.Configuration)
          .CreateLogger();

        // Application Services
        builder.Services.AddMediatR(opts => opts.RegisterServicesFromAssembly(typeof(Program).Assembly));
        builder.Services.AddIPathApplication(builder.Configuration);
        // builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        // Custom Authentication
        builder.Services.AddAuthenticationCore();
        builder.Services.AddScoped<ProtectedBrowserStorage, ProtectedSessionStorage>();
        builder.Services.AddScoped<AuthenticationStateProvider, iPathAuthenticationStateProvider>();

        // ViewModels
        builder.Services.AddServerViewModels(builder.Configuration);

        // Controllers
        builder.Services.AddControllers();


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapControllers();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}