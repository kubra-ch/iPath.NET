using iPath.Application.Features;
using iPath.Application.Querying;
using iPath.Data.Entities;
using iPath.UI.Areas.DataAccess;
using MudBlazor;

namespace iPath.UI.Components.Admin;

public class UserAdminViewModel(IDataAccess srvData, ISnackbar snackbar)
{

    public async Task<TableData<Community>> GetCommunityTableData(TableState state, string searchString, CancellationToken token)
    {
        var request = new GetCommuniyListQuery
        {
            Page = state.Page,
            PageSize = state.PageSize,
            UserId = null
        };

        if (!string.IsNullOrEmpty(searchString))
        {
            request.AddFilter("Name", searchString);
        }

        if (!string.IsNullOrEmpty(state.SortLabel))
        {
            request.AddSorting(state.SortLabel, (state.SortDirection == SortDirection.Ascending));
        }

        var resp = await srvData.Send(request);

        return new TableData<Community>() { TotalItems = resp.TotalItemsCount, Items = resp.Data };
    }


}
