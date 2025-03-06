using iPath.Application.Features;
using iPath.Application.Querying;
using iPath.Data.Entities;
using iPath.UI.Areas.DataAccess;
using iPath.UI.Components.Groups.Dialogs;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace iPath.UI.Components.Groups;

public class GroupAdminViewModel(IDataAccess srvData, ISnackbar snackbar, IDialogService srvDialog, IStringLocalizer T)
{
    private Group _selectedGroup;
    public Group SelectedGroup
    {
        get => _selectedGroup;
        set => _selectedGroup = value;
    }


    public bool EditDisabled => _selectedGroup == null;


    public async Task LoadSelectedGroup(int Id)
    {
        var resp = await srvData.Send(new GetGroupQuery(Id));
        if (resp.Success)
        {
            _selectedGroup = resp.Data;
        }
        else
        {
            snackbar.Add(resp.Message, Severity.Error);
        }
    }


    public async Task<TableData<GroupDTO>> GetTableData(TableState state, Community community, CancellationToken token)
    {
        if( community is null)
        {
            return new TableData<GroupDTO> { TotalItems = 0 };
        }


        var request = new GetGroupListQuery
        {
            CommunityId = community.Id,
            Page = state.Page,
            PageSize = state.PageSize,
            UserId = null
        };


        if (!string.IsNullOrEmpty(state.SortLabel))
        {
            request.AddSorting(state.SortLabel, state.SortDirection == SortDirection.Ascending);
        }

        var resp = await srvData.Send(request);

        if (!resp.Success) snackbar.Add(resp.Messagae, Severity.Error);

        return new TableData<GroupDTO>() { TotalItems = resp.TotalItemsCount, Items = resp.Data };
    }

    public string SelectedRowClassFunc(GroupDTO grp, int rowNumber)
    {
        if (grp != null && SelectedGroup != null && grp.Id == SelectedGroup.Id)
        {
            return "selected";
        }
        return String.Empty;
    }



    public async Task<GroupCommandResponse> CreateGroup(Community community)
    {
        var model = new CreateGroupCommand { CommunityId = community.Id };
        var parameters = new DialogParameters<CreateGroupDialog> { { x => x.Model, model } };
        var dialog = await srvDialog.ShowAsync<CreateGroupDialog>(T["Create Group"], parameters);
        var result = await dialog.Result;

        if (!result.Canceled && result.Data != null)
        {
            var cmd = (CreateGroupCommand)result.Data;
            var resp = await srvData.Send(cmd);
            if (resp.Success)
            {
                snackbar.Add(T["Group created"], Severity.Success);
            }
            else
            {
                snackbar.Add(resp.Message, Severity.Error);
            }
            return resp;
        }

        return new GroupCommandResponse(false);
    }



    public async Task<GroupCommandResponse> UpdateGroup()
    {
        if (SelectedGroup == null) return new GroupCommandResponse(false);

        var model = new UpdateGroupCommand
        {
            Id = SelectedGroup.Id,
            Name = SelectedGroup.Name,
            Purpose = SelectedGroup.Settings.Purpose,
            OwnerId = SelectedGroup.OwnerId
        };

        var parameters = new DialogParameters<UpdateGroupDialog> { { x => x.Model, model } };
        var dialog = await srvDialog.ShowAsync<UpdateGroupDialog>(T["Update Group: {0}", model.Name], parameters);
        var result = await dialog.Result;

        if (!result.Canceled && result.Data != null)
        {
            var cmd = (UpdateGroupCommand)result.Data;
            var resp = await srvData.Send(cmd);
            if (resp.Success)
            {
                snackbar.Add(T["Group updated"], Severity.Success);
            }
            else
            {
                snackbar.Add(resp.Message, Severity.Error);
            }
            return resp;
        }

        return new GroupCommandResponse(false);
    }



    public async Task<bool> DeleteGroup()
    {
        if (SelectedGroup != null)
        {
            var result = srvDialog.ShowMessageBox(T["Delete"], T["Are you sure that you want to delete the Group {0}", SelectedGroup.Name], T["Yes"], T["No"]);
            if (result != null)
            {
                var resp = await srvData.Send(new DeleteGroupCommand(Id: SelectedGroup.Id));
                if (resp.Success) return true;
                snackbar.Add(resp.Message, Severity.Error);
            }
        }
        return false;
    }
}

