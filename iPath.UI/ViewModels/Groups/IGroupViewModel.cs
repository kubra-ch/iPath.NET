using iPath.Application.Features;
using iPath.Data.Entities;
using iPath.UI.ViewModels.Nodes;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Groups;

public interface IGroupViewModel
{
    Task LoadGroupAsync(int Id);

    GroupModel Model { get; }

    GridItemsProvider<NodeModel> NodesDataProvider { get; }

    string ErrorMessage { get; }

    Task<NodeCommandRespone> CreateNodeAsync(int UserId, string Title, NodeType nType);
}
