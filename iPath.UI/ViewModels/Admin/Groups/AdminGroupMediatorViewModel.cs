using iPath.Application.Features;
using iPath.Application.Querying;
using iPath.Data.Entities;
using MediatR;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Admin.Groups;

public class AdminGroupMediatorViewModel(IMediator mediator) : IAdminGroupViewModel
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

            var result = await mediator.Send(request);

            return GridItemsProviderResult.From(
                items: result.Items,
                totalItemCount: result.TotalItemsCount
                );
        };
    }

    private GridItemsProvider<Group> _GridDataProvider = default!;
    public GridItemsProvider<Group> GridDataProvider => _GridDataProvider;



    public async Task SelectGroupId(int Id)
    {
        var resp = await mediator.Send(new GetGroupQuery(GroupId: Id));
        _selectedGroup = resp.Item;
    }

    private Group _selectedGroup = null;
    public Group SelectedGroup => _selectedGroup;



    public async Task<CreateGroupResponse> CreateGroupAsync(string Name, string? purpose, int? ownerId, Community? community)
    {
        var request = new CreateGroupCommand { Name = Name, Purpose = purpose, OwnerId = ownerId, CommunityId = community?.Id };
        var response = await mediator.Send(request);
        if (response.Success)
        {
            if (community is not null)
            {
                community.Groups.Add(response.Item);
            }
        }
        else
        {
            _errorMessage = response.Message;
        }
        return response;
    }


    public async Task<UpdateGroupResponse> UpdateGroupAsync(Group item)
    {
        var request = new UpdateGroupCommand() { Item = item };
        return await mediator.Send(request);
    }
}
