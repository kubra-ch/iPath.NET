using iPath.Application.Features;
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


        // viewmodels
        services.AddScoped<IAdminUserViewModel, AdminUserMediatorViewModel>();
        services.AddScoped<IAdminCommunityViewModel, AdminCommunityMediatorViewModel>();
        services.AddScoped<IAdminGroupViewModel, AdminGroupMediatorViewModel>();
        services.AddScoped<IUserMemberViewModel, UserMemberViewModel>();
        services.AddScoped<IGroupListViewModel, GroupListViewModelMediator>();
        services.AddScoped<IGroupViewModel, GroupViewModelMediator>();
        services.AddScoped<INodeViewModel, NodeViewModelMediator>();

        // UI services
        services.AddScoped<ClipboardService>();

        return services;
    }
}

