using iPath.Application.Features;
using iPath.UI.Areas.DataAccess;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Groups;

public class GroupListViewModel
    (IDataAccess srvData) : IGroupListViewModel
{
    public string SearchTerm { get; set; }
    public int? UserId { get; set; }
    public int? CommunityId { get; set; }


    private string _error;
    public string ErrorMessage => _error;

    private GridItemsProvider<GroupListDTO> _GridDataProvider = default!;
    public GridItemsProvider<GroupListDTO> GridDataProvider => _GridDataProvider;


    public async Task ExecuteSearchAsync()
    {
        var request = new GetGroupListDtoQuery()
        {
            UserId = this.UserId,
            CommunityId = this.CommunityId,
        };

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            request.Filter ??= new();
            request.Filter.AddFilter("Name", SearchTerm);
        }

        // get datae
        _GridDataProvider = async req =>
        {
            request.StartIndex = req.StartIndex;
            request.Count = req.Count;

            var result = (await srvData.Send(request)).Data;

            return GridItemsProviderResult.From(
                items: result.Items,
                totalItemCount: result.TotalItemsCount
                );
        };
    }
}
