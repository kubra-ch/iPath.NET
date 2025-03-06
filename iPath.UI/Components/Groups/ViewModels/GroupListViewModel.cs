using iPath.Application.Features;
using iPath.Application.Querying;
using iPath.UI.Areas.AppState;
using iPath.UI.Areas.DataAccess;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace iPath.UI.Components.Groups;

public class GroupListViewModel(IDataAccess srvData)
{
    public int? UserId;


    public async Task<TableData<GroupDTO>> GetTableData(TableState state, string searchString, CancellationToken token)
    {
        if (!UserId.HasValue) return new TableData<GroupDTO> ();

        var request = new GetGroupListQuery {
            UserId = UserId,            
            IncludeNodeCount = true
        };

        if( !string.IsNullOrEmpty(searchString))
        {
            request.AddFilter("Name", searchString);
        }

        if( !string.IsNullOrEmpty(state.SortLabel))
        {
            request.AddSorting(state.SortLabel, state.SortDirection == SortDirection.Ascending);
        }

        var resp = await srvData.Send(request);

        return new TableData<GroupDTO>() { TotalItems = resp.TotalItemsCount, Items = resp.Data };
    }

}
