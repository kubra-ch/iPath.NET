using iPath.Application.Features;
using iPath.Data.Entities;
using iPath.UI.Areas.DataAccess;

namespace iPath.UI.Components.Nodes.ViewModels;

public class NodeListViewModel(IDataAccess srvData)
{

    public async Task<NodeCommandResponse> CreateNewNode(Node newNode)
    {
        return await srvData.Send(new CreateNodeCommand(node: newNode));
    }

}
