using iPath.Application.Areas.Authentication;
using iPath.Application.Services;
using iPath.Application.SnomedCT;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Microsoft.AspNetCore.Builder;
using iPath.Application.DbSeeding;
using iPath.Application.Services.Storage;
using iPath.Application.Authentication;
using iPath.Application.Services.Cache;
using iPath.Application.Localization;
using Microsoft.Extensions.Localization;

namespace iPath.Application;

public static class DI_Application
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Mediator
        services.AddMediatR(opts => opts.RegisterServicesFromAssembly(typeof(DI_Application).Assembly));

        // Add Authentication
        services.AddIPathAuthentication(configuration);


        // Localization
        services.Configure<LocalizationSettings>(configuration.GetSection("LocalizationSettings"));
        // register the translation service as singleton for direct access
        services.AddSingleton<LocalizationService>();
        // register the same singleton for use as IStringLocalizer
        services.AddSingleton<IStringLocalizer>(p => p.GetRequiredService<LocalizationService>());



        // Snomed FHIR Client
        var SnomedUrl = "http://basyssrvdock1:8082/fhir/";

        services.AddHttpClient("SnomedFhir", client=>
        {
            client.BaseAddress = new Uri(SnomedUrl);
        })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                // FhirClient configures its internal HttpClient this way
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip                
            });

        services.AddScoped<SnomedClient>();

        services.AddTransient<IThumbImageService, ThumbImageService>();
        services.AddTransient<IStorageService, LocalStorageService>();

        // json export
        services.AddSingleton<NodeJsonExportService>();
        services.AddHostedService(p => p.GetRequiredService<NodeJsonExportService>());

        // DB Seeder
        services.AddTransient<DbSeeder>();

        // caching
        services.AddMemoryCache();
        services.AddScoped<IDataCache, DataCache>();

        return services;
    }


    public static WebApplication SeedDatase(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
            seeder.UpdateDatabase();
            seeder.Seed();
        }
        return app;
    }

}
