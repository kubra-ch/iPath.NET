using iPath.Application.Features;
using iPath.Data.Entities;
using iPath.UI.ViewModels.Nodes;
using MediatR;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Groups;

public class GroupViewModelMediator(IMediator mediator) : IGroupViewModel
{
    private GroupModel _model;
    public GroupModel Model => _model;

    private GridItemsProvider<NodeModel> _NodesDataProvider = default!;
    public GridItemsProvider<NodeModel> NodesDataProvider => _NodesDataProvider;

    private string _error = default!;
    public string ErrorMessage => _error;

    public async Task LoadGroupAsync(int Id)
    {
        var rg = new GetGroupQuery(GroupId: Id);
        var respg = await mediator.Send(rg);
        if ( !respg.Success )
        {
            _error = respg.Message;
        }

        _model = new GroupModel()
        {
            Id = respg.Item.Id,
            Name = respg.Item.Name,
            Owner = respg.Item.Owner is null ? "" : respg.Item.Owner.Username,
            Purpose = respg.Item.Purpose
        };


        // get nodes
        var request = new GetNodeListQuery()
        {
            GroupId = Id,
        };

        // get datae
        _NodesDataProvider = async req =>
        {
            request.StartIndex = req.StartIndex;
            request.Count = req.Count;

            var result = await mediator.Send(request);

            var models = new List<NodeModel>();
            foreach (var node in result.Items)
            {
                models.Add(new NodeModel(node));
            }


            return GridItemsProviderResult.From(
                items: models,
                totalItemCount: result.TotalItemsCount
                );
        };
    }

    public async Task<NodeCommandRespone> CreateNodeAsync(int UserId, string Title, NodeType nodeType)
    {
        var req = new CreateNodeCommand()
        {
            OwnerId = UserId,
            GroupId = Model.Id,
            NodeType = nodeType,
            Title = Title 
        };
        return await mediator.Send(req);
    }
}
