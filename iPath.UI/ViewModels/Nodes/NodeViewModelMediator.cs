using iPath.Application.Features;
using iPath.Application.Features.Nodes.Commands;
using iPath.Data.Entities;
using MediatR;

namespace iPath.UI.ViewModels.Nodes;

public class NodeViewModelMediator(IMediator mediator, ILogger<NodeViewModelMediator> logger) : INodeViewModel
{
    private NodeModel _model;
    public NodeModel Model => _model;

    // Gallery Browsing
    public NodeModel ActiveChild { get; set; } = null!;
    public void SelectChild(int ChildId)
    {
        ActiveChild = Model != null ? Model.Children.FirstOrDefault(c => c.Id == ChildId) : null!;
        CollapseDescritpion = ActiveChild != null;
    }

    public void PreviousImage() 
    {
        if (Model != null && ActiveChild != null)
        {
            var prevList = Model.Children.ToList();
            prevList.Reverse();
            ActiveChild = prevList.SkipWhile(x => x.Id != ActiveChild.Id).Skip(1).FirstOrDefault();
        }
        else
        {
            ActiveChild = null!;
        }
    }

    public void NextImage() 
    {
        if (Model != null && ActiveChild != null)
        {
            ActiveChild = Model.Children.SkipWhile(x => x.Id != ActiveChild.Id).Skip(1).FirstOrDefault();
        }
        else
        {
            ActiveChild = null!;
        }
    }


    // Display Helper
    public string SubTitleText => Model != null && !Model.HasSubTitle ? "Description" : Model.SubTitle;
    public bool CollapseDescritpion { get; set; }



    private string _message = default!;
    public string Message => _message;

    public async Task LoadNode(int NodeId)
    {
        ActiveChild = null;
        var resp = await mediator.Send(new GetNodeQuery { Id = NodeId });
        if( resp != null)
        {
            _message = "";
            _model = new NodeModel(resp);
        }
        else
        {
            _model = null;
            _message = $"Node #{NodeId} not found";
        }
    }

    public async Task<UploadNodeFileResponse> UploadFileAsync(int UserId, string Filename, FileInfo localFile)
    {
        if (Model is null) return new UploadNodeFileResponse(false);

        if (localFile == null || !localFile.Exists ) return new UploadNodeFileResponse(false, null, "upload not found");

        logger.LogInformation("starting file upload: " + Filename);

        var resp = await mediator.Send(new UploadNodeFileCommand(NodeId: Model.Id, UserId: UserId, filename: Filename, localFilePath: localFile.FullName));

        return resp;
    }

    public async Task<AddNodeAnnotationResponse> AddAnnotationAsync(int UserId, string text)
    {
        if (Model is null) return new AddNodeAnnotationResponse(false);

        var req = new AddNodeAnnotationCommand(NodeId: Model.Id, UserId: UserId, text: text);
        var resp = await mediator.Send(req);

        if( resp.Success)
        {
            Model.Annotations.Add(new AnnotationModel(resp.item));
        }

        return resp;
    }

    public async Task<NodeCommandRespone> DeleteNodeAsync(int NodeId)
    {
        return await mediator.Send(new DeleteNodeCommand(NodeId: NodeId));
    }

    public async Task<NodeCommandRespone> DeleteNodesAsync(List<int> NodeIds)
    {
        return await mediator.Send(new DeleteNodesCommand(NodeIds: NodeIds));
    }

    public async Task<NodeCommandRespone> UpdateNodeVisibilityAsync(int NodeId, eNodeVisibility newValue)
    {
        return await mediator.Send(new UpdateNodeVisibilityCommand(NodeId: NodeId, newValue: newValue));
    }

    public async Task<NodeCommandRespone> UpdateNodesVisibilityAsync(List<int> NodeIds, eNodeVisibility newValue)
    {
        return await mediator.Send(new UpdateNodesVisibilityCommand(NodeIds: NodeIds, newValue: newValue));
    }

    public async Task<NodeCommandRespone> UpdateNodeAsync()
    {
        if (Model is null) return new NodeCommandRespone(false);

        var request = new UpdateNodeCommand();
        request.Id = Model.Id;
        request.Title = Model.Title;
        request.SubTitle = Model.SubTitle;
        request.Description = Model.Description;
        request.Status = Model.Status;
        request.Visibility = Model.Visibility;

        return await mediator.Send(request);
    }
}
