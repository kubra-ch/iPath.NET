using iPath.Application.Features;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Admin.Communities;

public interface IAdminCommunityViewModel
{
    bool IsReady { get; }

    GridItemsProvider<CommunityDto> GridDataProvider { get; }

    Task<List<CommunityDto>> FindCommunityAsync(string term);
    Task<List<CommunityDto>> GetAllCommunityAsync();

    string SearchTerm { get; set; }
    Task ExecuteSearchAsync();

    Task SelectCommunityId(int Id);
    CommunityModel SelectedCommunity { get; }


    Task<int> GetCommunityCountAsync();
    Task<CommunityCommandResponse> CreateCommunityAsync(string Name);
    // Task<UpdateCommunityResponse> UpdateCommunityAsync(Community item);
}
