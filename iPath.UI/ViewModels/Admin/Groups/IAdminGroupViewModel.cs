using iPath.Application.Features;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Admin.Groups;

public interface IAdminGroupViewModel
{
    string ErrorMessage { get; }

    GridItemsProvider<GroupDto> GridDataProvider { get; }

    string SearchTerm { get; set; }
    Task ExecuteSearchAsync();

    Task SelectGroupId(int Id);
    GroupModel SelectedGroup { get; }

    Task<GroupCommandResponse> CreateGroupAsync(string Name, string? Purpose, int? OwnerId, CommunityModel? community);
    Task<GroupCommandResponse> UpdateGroupAsync(GroupDto item);
}
