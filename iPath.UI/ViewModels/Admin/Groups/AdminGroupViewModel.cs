using iPath.Application.Features;
using iPath.Application.Querying;
using iPath.Data.Entities;
using iPath.UI.Areas.DataAccess;
using MediatR;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Admin.Groups;

public class AdminGroupViewModel(IDataAccess srvData) : IAdminGroupViewModel
{
    private string _errorMessage = string.Empty;
    public string ErrorMessage => _errorMessage;


    public string SearchTerm { get; set; } = default!;

    public async Task ExecuteSearchAsync()
    {
        var request = new GetGroupListQuery(); ;

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
            request.SortDefinitions ??= new();
            request.SortDefinitions.Add(new SortDefinition { SortColumn = "Name" });

            var response = await srvData.Send(request);
            if (!response.Success)
            {
                _errorMessage = response.Message;
                throw new Exception(response.Message);
                return GridItemsProviderResult.From(items: new List<Group>(), totalItemCount: 0);
            }

            return GridItemsProviderResult.From(
                items: response.Data.Items,
                totalItemCount: response.Data.TotalItemsCount
                );
        };
    }

    private GridItemsProvider<Group> _GridDataProvider = default!;
    public GridItemsProvider<Group> GridDataProvider => _GridDataProvider;



    public async Task SelectGroupId(int Id)
    {
        var resp = await srvData.Send(new GetGroupQuery(GroupId: Id));
        _selectedGroup = resp.Data;
    }

    private Group _selectedGroup = null;
    public Group SelectedGroup => _selectedGroup;



    public async Task<GroupCommandResponse> CreateGroupAsync(string Name, string? purpose, int? ownerId, Community? community)
    {
        var request = new CreateGroupCommand(Name: Name, Purpose: purpose, OwnerId: ownerId, CommunityId: community?.Id);
        var response = await srvData.Send(request);
        if (response.Success)
        {
            if (community is not null)
            {
                community.Groups.Add(response.Data);
            }
        }
        else
        {
            _errorMessage = response.Message;
        }
        return response;
    }


    public async Task<GroupCommandResponse> UpdateGroupAsync(Group item)
    {
        var request = new UpdateGroupCommand(item);
        return await srvData.Send(request);
    }
}
