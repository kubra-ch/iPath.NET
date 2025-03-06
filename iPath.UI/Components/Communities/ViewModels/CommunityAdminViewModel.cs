using iPath.Application.Features;
using iPath.Application.Querying;
using iPath.Data.Entities;
using iPath.UI.Areas.DataAccess;
using iPath.UI.Components.Communities.Dialogs;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace iPath.UI.Components.Communities;

public class CommunityAdminViewModel(IDataAccess srvData, ISnackbar snackbar, IDialogService srvDialog, IStringLocalizer T)
{
    private Community _selectedCommunity;
    public Community SelectedCommunity
    {
        get => _selectedCommunity;
        set => _selectedCommunity = value;
    }

    public async Task LoadSelectedCommunity(int Id)
    {
        var resp = await srvData.Send(new GetCommuniyQuery(Id));
        if (resp.Success)
        {
            _selectedCommunity = resp.Data;
        }
        else
        {
            snackbar.Add(resp.Message, Severity.Error);
        }
    }


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
            request.AddSorting(state.SortLabel, state.SortDirection == SortDirection.Ascending);
        }

        var resp = await srvData.Send(request);

        if (!resp.Success) snackbar.Add(resp.Messagae, Severity.Error);

        return new TableData<Community>() { TotalItems = resp.TotalItemsCount, Items = resp.Data };
    }

    public string SelectedRowClassFunc(Community com, int rowNumber)
    {
        if (com != null && SelectedCommunity != null && com.Id == SelectedCommunity.Id)
        {
            return "selected";
        }
        return String.Empty;
    }




    public async Task<CommunityCommandResponse> CreateCommunity()
    {
        var dialog = await srvDialog.ShowAsync<CreateCommunityDialog>(T["Create Community"]);
        var result = await dialog.Result;

        if (!result.Canceled && result.Data != null)
        {
            var cmd = (CreateCommunityCommand)result.Data;
            var resp = await srvData.Send(cmd);
            if (resp.Success)
            {
                snackbar.Add(T["Community create"], Severity.Success);
            }
            else
            {
                snackbar.Add(resp.Message, Severity.Error);
            }
            return resp;
        }

        return new CommunityCommandResponse(false);
    }


    public bool EditDisabled => SelectedCommunity == null;


    public async Task<CommunityCommandResponse> UpdateCommunity()
    {
        if (SelectedCommunity == null) return new CommunityCommandResponse(false);

        var model = new UpdateCommunityCommand
        {
            Id = SelectedCommunity.Id,
            Name = SelectedCommunity.Name,
            Description = SelectedCommunity.Description,
            OwnerId = SelectedCommunity.OwnerId
        };

        var parameters = new DialogParameters<UpdateCommunityDialog> { { x => x.Model, model } };
        var dialog = await srvDialog.ShowAsync<UpdateCommunityDialog>(T["Update Community: {0}", model.Name]);
        var result = await dialog.Result;

        if (!result.Canceled && result.Data != null)
        {
            var cmd = (UpdateCommunityCommand)result.Data;
            var resp = await srvData.Send(cmd);
            if (resp.Success)
            {
                snackbar.Add(T["Community updated"], Severity.Success);
            }
            else
            {
                snackbar.Add(resp.Message, Severity.Error);
            }
            return resp;
        }

        return new CommunityCommandResponse(false);
    }



    public async Task<bool> DeleteCommunity()
    {
        if (SelectedCommunity != null)
        {
            var result = srvDialog.ShowMessageBox(T["Delete"], T["Are you sure that you want to delete the community {0}", SelectedCommunity.Name], T["Yes"], T["No"]);
            if (result != null)
            {
                var resp = await srvData.Send(new DeleteCommunityCommand(Id: SelectedCommunity.Id));
                if (resp.Success) return true;
                snackbar.Add(resp.Message, Severity.Error);
            }
        }
        return false;
    }
}

