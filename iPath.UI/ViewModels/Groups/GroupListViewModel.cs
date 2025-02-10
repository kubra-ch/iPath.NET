using iPath.Application.Features;
using iPath.Application.Querying;
using iPath.UI.Areas.DataAccess;
using Microsoft.FluentUI.AspNetCore.Components;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                request.SortDefinitions.Add(new SortDefinition { SortColumn = "Name", SortAscending = true });
            }

            var result = (await srvData.Send(request)).Data;

            return GridItemsProviderResult.From(
                items: result.Items,
                totalItemCount: result.TotalItemsCount
                );
        };
    }
}
