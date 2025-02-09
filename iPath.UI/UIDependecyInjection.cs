using iPath.Application.Features;
using iPath.UI.Areas.DataAccess;
using iPath.UI.Areas.DraftStorage;
using iPath.UI.Componenets.Code;
using iPath.UI.ViewModels.Admin.Communities;
using iPath.UI.ViewModels.Admin.Groups;
using iPath.UI.ViewModels.Admin.Users;
using iPath.UI.ViewModels.Groups;
using iPath.UI.ViewModels.Nodes;

namespace iPath.UI;

public static class UIDependecyInjection
{
    public static IServiceCollection AddServerViewModels(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(opts => opts.RegisterServicesFromAssemblyContaining<GetUserQuery>());


        // data access
        // Blazor Server over Mediator
        services.AddScoped<IDataAccess, DataAccessMediator>();
        // Blazor WASM over API
        // services.AddHttpClient();
        // services.AddScoped<IDataAccess, DataAccessREST>();

        // Drafts 
        services.AddScoped<IDraftStore, DraftServiceBlazorStore>();

        // viewmodels
        services.AddScoped<IAdminUserViewModel, AdminUserViewModel>();
        services.AddScoped<IAdminCommunityViewModel, AdminCommunityViewModel>();
        services.AddScoped<IAdminGroupViewModel, AdminGroupViewModel>();
        services.AddScoped<IUserMemberViewModel, UserMemberViewModel>();
        services.AddScoped<IGroupListViewModel, GroupListViewModel>();
        services.AddScoped<IGroupViewModel, GroupViewModelMediator>();
        services.AddScoped<INodeViewModel, NodeViewModel>();

        // UI services
        services.AddScoped<ClipboardService>();

        return services;
    }
}

