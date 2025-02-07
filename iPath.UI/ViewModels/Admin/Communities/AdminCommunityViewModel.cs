using iPath.Application.Features;
using iPath.Application.Querying;
using iPath.Data.Entities;
using iPath.UI.ViewModels.DataService;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Admin.Communities;

public class AdminCommunityViewModel(IDataAccess srvData) : IAdminCommunityViewModel
{
    private bool _IsReady = false;
    public bool IsReady => _IsReady;

    public string SearchTerm { get; set; } = default!;

    public async Task ExecuteSearchAsync()
    {
        var request = new GetCommunityListQuery(); ;

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            request.Filter ??= new();
            request.Filter.AddFilter("name", SearchTerm);
        }

        // get datae
        _GridDataProvider = async req =>
        {
            request.StartIndex = req.StartIndex;
            request.Count = req.Count;
            request.SortDefinitions ??= new();
            request.SortDefinitions.Add(new SortDefinition { SortColumn = "name" });

            var response = await srvData.Send(request);
            _IsReady = true;

            if (!response.Success)
                throw new Exception(response.Message);

            return GridItemsProviderResult.From(
                items: response.Data.Items,
                totalItemCount: response.Data.TotalItemsCount
                );
        };

    }

    private GridItemsProvider<Community> _GridDataProvider = default!;
    public GridItemsProvider<Community> GridDataProvider => _GridDataProvider;



    public async Task SelectCommunityId(int Id)
    {
        var response = await srvData.Send(new GetCommunityQuery(Id));
        _selectedCommunity = response.Data;
    }

    private Community _selectedCommunity = null;
    public Community SelectedCommunity => _selectedCommunity;


    public async Task<int> GetCommunityCountAsync()
    {
        var response = await srvData.Send(new GetCommunityListQuery());
        if (!response.Success) throw new Exception(response.Message);

        _IsReady = true;
        return response.Data.TotalItemsCount;
    }

    public async Task<CommunityCommandResponse> CreateCommunityAsync(string name)
    {
        return await srvData.Send(new CreateCommunityCommand(Name: name));
    }
}
