using iPath.Application.Features;
using iPath.Data.Entities;
using MediatR;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Groups;

public interface IGroupListViewModel
{
    string ErrorMessage { get; }

    GridItemsProvider<GroupListDto> GridDataProvider { get; }

    string SearchTerm { get; set; }
    int? UserId { get; set; }
    int? CommunityId { get; set; }

    Task ExecuteSearchAsync();
}
