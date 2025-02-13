using iPath.Application.Features;
using iPath.Application.Querying;
using iPath.UI.Areas.DataAccess;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Admin.Communities;

public class AdminCommunityViewModel(IDataAccess srvData) : IAdminCommunityViewModel
{
    private bool _IsReady = false;
    public bool IsReady => _IsReady;

    public string SearchTerm { get; set; } = default!;


    public async Task<List<CommunityDto>> FindCommunityAsync(string term)
    {
        var request = new GetCommunityListQuery();

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            request.Filter ??= new();
            request.Filter.AddFilter("Name", SearchTerm);
        }

        request.StartIndex = 0;
        request.Count = 100;
        request.SortDefinitions ??= new();
        request.SortDefinitions.Add(new SortDefinition { SortColumn = "Name" });

        var response = await srvData.Send(request);
        if (!response.Success) throw new Exception(response.Message);
        return response.Data.Items;
    }

    public async Task<List<CommunityDto>> GetAllCommunityAsync()
    {
        var request = new GetCommunityListQuery();
        request.StartIndex = 0;
        request.Count = 0;
        request.SortDefinitions ??= new();
        request.SortDefinitions.Add(new SortDefinition { SortColumn = "Name" });
        var response = await srvData.Send(request);
        if (!response.Success) throw new Exception(response.Message);
        return response.Data.Items;
    }



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

            // sorting
            request.SortDefinitions = new();
            var sort = req.GetSortByProperties();
            if (sort != null && sort.Any())
            {
                foreach (var p in sort)
                {
                    var sd = new SortDefinition { SortColumn = p.PropertyName, SortAscending = (p.Direction == SortDirection.Ascending) };
                    request.SortDefinitions.Add(sd);
                }
            }
            else
            {
                request.SortDefinitions.Add(new SortDefinition { SortColumn = "CreatedOn", SortAscending = false });
            }

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

    private GridItemsProvider<CommunityDto> _GridDataProvider = default!;
    public GridItemsProvider<CommunityDto> GridDataProvider => _GridDataProvider;



    public async Task SelectCommunityId(int Id)
    {
        var response = await srvData.Send(new GetCommunityQuery(Id));

        // get groups
        var groupResp = await srvData.Send(new GetGroupListQuery { CommunityId = Id, Count = 0 });

        _selectedCommunity = new CommunityModel(response.Data, groupResp.Data.Items);
    }

    private CommunityModel _selectedCommunity = null;
    public CommunityModel SelectedCommunity => _selectedCommunity;


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
