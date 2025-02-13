using iPath.Application.Configuration;
using iPath.Application.Features;
using iPath.Application.Features.Nodes.Commands;
using iPath.Application.Services;
using iPath.Data.Database.Migrations;
using iPath.Data.Entities;
using iPath.UI.Areas.DataAccess;
using iPath.UI.Areas.DraftStorage;
using iPath.UI.ViewModels.Drafts;
using Microsoft.Extensions.Options;
using System.Threading.Tasks.Dataflow;

namespace iPath.UI.ViewModels.Nodes;

public class NodeViewModel(IDataAccess srvData, 
    IDraftStore draftStore, 
    ISessionStateService sessState, 
    IOptions<iPathConfig> opts,
    ILogger<NodeViewModel> logger)
    : INodeViewModel
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


    public void ResetData()
    {
        ActiveChild = null;
        _message = "";
        _model = null;
    }


    public async Task LoadNode(int NodeId)
    {
        ActiveChild = null;
        var resp = await srvData.Send(new GetNodeQuery(NodeId));
        if( resp.Success )
        {
            GroupListDto grp = null!;
            var grpR = await srvData.Send(new GetGroupQuery(resp.Data.GroupId.HasValue ? resp.Data.GroupId.Value : 0));
            _message = "";
            _model = new NodeModel(resp.Data, grpR.Data);
        }
        else
        {
            _model = null;
            _message = resp.Message;
        }
    }

    public async Task<NodeCommandRespone> UploadFileAsync(int UserId, string Filename, FileInfo localFile)
    {
        if (Model is null) return new NodeCommandRespone(false);

        if (localFile == null || !localFile.Exists ) return new NodeCommandRespone(false, "upload not found");

        logger.LogInformation("starting file upload: " + Filename);

        var resp = await srvData.Send(new UploadNodeFileCommand(NodeId: Model.Id, UserId: UserId, filename: Filename, localFilePath: localFile.FullName));

        return resp;
    }

    public async Task<NodeCommandRespone> DeleteNodeAsync(int NodeId)
    {
        return await srvData.Send(new DeleteNodeCommand(NodeId: NodeId));
    }

    public async Task<NodeCommandRespone> DeleteNodesAsync(List<int> NodeIds)
    {
        return await srvData.Send(new DeleteNodesCommand(NodeIds: NodeIds));
    }

    public async Task<NodeCommandRespone> UpdateNodeVisibilityAsync(int NodeId, eNodeVisibility newValue)
    {
        return await srvData.Send(new UpdateNodeVisibilityCommand(NodeId: NodeId, newValue: newValue));
    }

    public async Task<NodeCommandRespone> UpdateNodesVisibilityAsync(List<int> NodeIds, eNodeVisibility newValue)
    {
        return await srvData.Send(new UpdateNodesVisibilityCommand(NodeIds: NodeIds, newValue: newValue));
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

        return await srvData.Send(request);
    }

    public async Task<NodeCommandRespone> UpdateSortNumerbs(List<(int NodeId, int SortNr)> newOrder)
    {
        return await srvData.Send(new UpdateNodesSortNrSortNrCommand(newOrder));
    }

    public async Task<AnnotationCommandResponse> CreateAnnotationAsync(int UserId, string? text = null!)
    {
        if (Model is null) return new AnnotationCommandResponse(false);

        var resp = await srvData.Send(new CreateAnnotationCommand(NodeId: Model.Id, UserId: UserId, text: text));
        if (resp.Success) Model.AddAnnotation(resp.Data);
        return resp;
    }
    public async Task<AnnotationCommandResponse> CreateAnnotationAsync(CreateAnnotationDraft draft)
    {
        return await CreateAnnotationAsync(UserId: draft.User.Id, text: draft.Text);
    }


    public async Task<AnnotationCommandResponse> UpdateAnnotationAsync(AnnotationModel model, string? text = null!, eAnnotationVisibility? visibility = null!, int? userId = null!)
    {
        var resp = await srvData.Send(new UpdateAnnotationCommand(Id: model.Id, Text: text, Visibility: visibility, UserId: userId));
        if (resp.Success) model.LoadData(resp.Data);
        return resp;
    }

    public async Task<AnnotationCommandResponse> DeleteAnnotationAsync(AnnotationModel model)
    {
        var resp = await srvData.Send(new UpdateAnnotationCommand(Id: model.Id, Visibility: eAnnotationVisibility.Deleted));
        if (resp.Success)
        {
            if( resp.Data == null )
            {
                // remove from DB
                Model.Annotations.RemoveAll(a => a.Id == model.Id);
            }
            else
            {
                model.LoadData(resp.Data);
            }
        }

        return resp;
    }



    public IDraftStore DraftStore => draftStore;

    public async Task<CreateAnnotationDraft> GetAnnotationDraft(bool autoCreate)
    {
        var d = await draftStore.GetDraft<CreateAnnotationDraft>(CreateAnnotationDraft.NodeKey(Model.Id));
        if( d == null && autoCreate)
        {
            d = CreateAnnotationDraft.ForNode(Model.Id);
            d.User = sessState.User;
            draftStore.SetDraft(d);
        }
        return d;
    }



    public bool AnnotationIsVisible(AnnotationModel model)
    {
        return model != null && model.IsVisibleInSession(sessState);
    }

    public string ThumbUrl(NodeModel node)
    {        
        if (!string.IsNullOrEmpty(node.ThumbData))
        {
            return $"data:image/jpeg;base64, {node.ThumbData}";
        }
        else if (node.IsImage)
        {
            // return $"https://www.ipath-network.com/ipath/image/src/{nodeFile.Id}&thumb=1";
            var url =  $"{opts.Value.BaseUrl}api/files/thumb/{node.Id}";
            return url;
        }
        else if (node.NodeType == "folder")
        {
            return "https://www.ipath-network.com/ipath/images/folder.png";
        }
        else if (node.NodeType == "file" && node.Filename!.ToLower().EndsWith("pdf"))
        {
            return "https://www.ipath-network.com/ipath/images/pdf.png";
        }
        else
        {
            return "https://www.ipath-network.com/ipath/images/document.png";
        }

        return "";
    }

    public string FileUrl(NodeModel node)
    {
        if (node is null ) return "";
        return $"{opts.Value.BaseUrl}api/files/{node.Id}"; // + "/"  + HttpUtility.UrlEncode(this.Filename);
    }
}
