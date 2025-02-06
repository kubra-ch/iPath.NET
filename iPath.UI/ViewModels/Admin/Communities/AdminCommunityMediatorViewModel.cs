using iPath.Application.Features;
using iPath.Application.Querying;
using iPath.Data.Entities;
using MediatR;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Admin.Communities;

public class AdminCommunityMediatorViewModel(IMediator mediator) : IAdminCommunityViewModel
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

            var result = await mediator.Send(request);
            _IsReady = true;

            return GridItemsProviderResult.From(
                items: result.Items,
                totalItemCount: result.TotalItemsCount
                );
        };

    }

    private GridItemsProvider<Community> _GridDataProvider = default!;
    public GridItemsProvider<Community> GridDataProvider => _GridDataProvider;



    public async Task SelectCommunityId(int Id)
    {
        _selectedCommunity = await mediator.Send(new GetCommunityQuery { Id = Id });
    }

    private Community _selectedCommunity = null;
    public Community SelectedCommunity => _selectedCommunity;


    public async Task<int> GetCommunityCountAsync()
    {
        var result = await mediator.Send(new GetCommunityListQuery());
        _IsReady = true;
        return result.TotalItemsCount;
    }

    public async Task<CommunityResponse> CreateCommunityAsync(string name)
    {
        return  await mediator.Send(new CreateCommunityCommand(Name: name));
    }
}
