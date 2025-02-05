using iPath.Application.Features;
using iPath.Data.Entities;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Admin.Groups;

public interface IAdminGroupViewModel
{
    string ErrorMessage { get; }

    GridItemsProvider<Group> GridDataProvider { get; }

    string SearchTerm { get; set; }
    Task ExecuteSearchAsync();

    Task SelectGroupId(int Id);
    Group SelectedGroup { get; }

    Task<CreateGroupResponse> CreateGroupAsync(string Name, string? Purpose, int? OwnerId, Community? community);
    Task<UpdateGroupResponse> UpdateGroupAsync(Group item);
}
