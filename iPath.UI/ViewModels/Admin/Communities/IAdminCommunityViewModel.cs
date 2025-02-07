using iPath.Application.Features;
using iPath.Data.Entities;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Admin.Communities;

public interface IAdminCommunityViewModel
{
    bool IsReady { get; }

    GridItemsProvider<Community> GridDataProvider { get; }
    

    string SearchTerm { get; set; }
    Task ExecuteSearchAsync();

    Task SelectCommunityId(int Id);
    Community SelectedCommunity { get; }


    Task<int> GetCommunityCountAsync();
    Task<CommunityCommandResponse> CreateCommunityAsync(string Name);
    // Task<UpdateCommunityResponse> UpdateCommunityAsync(Community item);
}
