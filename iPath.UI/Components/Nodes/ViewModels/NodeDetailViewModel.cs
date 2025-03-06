using iPath.Application.Features;
using iPath.Data.Entities;
using iPath.Application.Services.Cache;
using iPath.UI.Areas.DataAccess;
using iPath.UI.Components.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.ComponentModel;
using iPath.UI.Components.Nodes.Dialogs;


namespace iPath.UI.Components.Nodes.ViewModels;

public class NodeDetailViewModel(IDataAccess srvData, IDataCache cache,
    NavigationManager nm, IDialogService srvDialog, ISnackbar snackbar,
    ILogger<NodeDetailViewModel> logger)
    : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private Node _node;
    public NodeModel Model { get; private set; }

    public int? prevId { get; private set; }
    public int? nextId { get; private set; }

    private string _errorMessage = default;
    public string ErrorMessage => _errorMessage;



    public bool EditActive { get; set; }
    public bool EditEnabled { get; private set; }


    private async Task UpdateEditEnabledAsync()
    {
        EditEnabled = false;
        if (_node != null)
        {
            if (cAppState.authManager.IsAdmin())
            {
                // Admin => always
                EditEnabled = true;
            }
            else if (_node.GroupId.HasValue && await cAppState.authManager.IsModerator(_node.GroupId.Value))
            {
                // Group-Moderator
                EditEnabled = true;
            }
            else
            {
                // own node
                EditEnabled =  (Model.Owner.UserId == cAppState.SessionUser.UserId);
            }
        }
    }


    private CascadingAppState cAppState;

    public async Task LoadNodeAync(int NodeId, CascadingAppState appState)
    {
        cAppState = appState;

        _node = null;
        _errorMessage = "";
        SelectedNode = null;
        prevId = null;
        nextId = null;

        var resp = await srvData.Send(new GetNodeQuery(NodeId));
        if (resp.Success)
        {
            await LoadNodeData(resp.Data);
        }
        else
        {
            _errorMessage = resp.Message;
        }
        await UpdateEditEnabledAsync();
    }

    public async Task ReloadNodeAsync()
    {
        if( _node != null)
        {
            await LoadNodeAync(_node.Id, cAppState);
        }
    }

    private async Task LoadNodeData(Node node)
    {
        if( _node != null && node != null && _node.Id != node.Id)
        {
            throw new Exception("cannot load a different node");
        }

        _node = node;
        Model = await NodeModel.CreateModelAsync (node, cache);
        SelectedNode = Model;

        if (cAppState.activeNodeIds != null)
        {
            var curIdx = cAppState.activeNodeIds.IndexOf(node.Id);
            prevId = curIdx > 0 ? cAppState.activeNodeIds[curIdx - 1] : null;
            nextId = curIdx < cAppState.activeNodeIds.Count() - 1 ? cAppState.activeNodeIds[curIdx + 1] : null;
        }
    }


    public async Task SetNodeVisitedAync()
    {
        if (_node != null)
        {
            await srvData.Send(new InsertUserNodeVisitCommand(UserId: cAppState.SessionUser.UserId, _node.Id));
        }
    }


    public async Task<BaseResponse> SaveNodeDescription()
    {
        return await srvData.Send(new UpdateNodeDescriptionCommand(NodeId: _node.Id, Data: Model.Description));
    }
    public async Task<BaseResponse> SaveChildNodeSortOrder(Dictionary<int, int> newSort)
    {
        return await srvData.Send(new UpdateChildNodeSortOrderCommand(NodeId: _node.Id, sortOrder: newSort));
    }


    public async Task<BaseResponse> CreateNewNode(Node newNode)
    {
        return await srvData.Send(new CreateNodeCommand(node: newNode)); 
    }

    public async Task DeleteNodeAsync(NodeModel node)
    {
        if( Model != null && Model.ChildNodes.Contains(node))
        {
            // delete child node
            var resp = await srvData.Send(new DeleteNodeCommand(node.Id));
            if( !resp.Success)
            {
                await srvDialog.ShowMessageBox("Error", resp.Message);
            }
            else
            {
                Model.ChildNodes.Remove(node);
            }
        }
        else if( Model != null && Model.Equals(node) )
        {
            // remove parent node
            var resp = await srvData.Send(new DeleteNodeCommand(node.Id));
            if (!resp.Success)
            {
                await srvDialog.ShowMessageBox("Error", resp.Message);
            }
        }

    }



    private long maxFileSize = 1024 * 1024 * 25;
    public async Task<bool> UploadFileAsync(IBrowserFile f)
    {
        if (Model is null) return false;

        logger.LogInformation("starting file upload: " + f.Name);

        var tmpFile = Path.GetTempFileName();
        await using FileStream fs = new(tmpFile, FileMode.Create);
        await f.OpenReadStream(maxFileSize).CopyToAsync(fs);

        var resp = await srvData.Send(new UploadNodeFileCommand(RootNodeId: _node.Id, ParentNodeId: Model.Id, UserId: cAppState.SessionUser.UserId, filename: f.Name, localFilePath: tmpFile));

        if( !resp.Success)
        {
            logger.LogError(resp.Message);
        }

        fs.Close();


        if(System.IO.File.Exists(tmpFile))
        {
            System.IO.File.Delete(tmpFile);
        }

        return resp.Success;
    }


    public async Task ShowProfileAsync()
    {
        if( Model != null )
        {
            await srvDialog.ShowProfile(await cache.GetProfileAsync(Model.Owner.UserId));
        }
    }



    #region "-- Child Node Handling --"
    public bool IsRootNodeSelected => _node is null || SelectedNode is null || SelectedNode.Id == _node.Id;


    public void GoNext()
    {
        if (!IsRootNodeSelected)
        {
            // next image inside Node
            SelectedNode = Model.GetSibling(SelectedNode, true);
        }
        else if( nextId.HasValue )
        {
            // next node in node list
            nm.NavigateTo($"node/{nextId}");
        }
    }

    public void GoPrevious()
    {
        if (!IsRootNodeSelected)
        {
            // previous image inside Node
            SelectedNode = Model.GetSibling(SelectedNode, false);
        }
        else if (prevId.HasValue)
        {
            // previous node in node list
            nm.NavigateTo($"node/{prevId}");
        }
    }

    public void GoUp()
    {
        if (SelectedNode != null && SelectedNode.ParentId.HasValue )
        {
            SelectedNode = Model.GetParent(SelectedNode);
        }
        else if (cAppState.activeGroupId.HasValue)
        {
            nm.NavigateTo($"group/{cAppState.activeGroupId}");
        }
        else
        {
            // my node list
            nm.NavigateTo($"nodes");
        }
    }


    private NodeModel _SelectedChildNode;

    public NodeModel SelectedNode { 
        get => _SelectedChildNode; 
        set
        {
            _SelectedChildNode = value;
            OnPropertyChanged(nameof(SelectedNode));
            OnPropertyChanged(nameof(IsRootNodeSelected));
        }
    }


    public string ThumbUrl(NodeModel node)
    {
        if (!string.IsNullOrEmpty(node.File?.ThumbData))
        {
            return $"data:image/jpeg;base64, {node.File.ThumbData}";
        }
        else
        {
            return $"https://www.ipath-network.com/ipath/image/src/{node.Id}";
        }
        
        /*
        if (node.File.IsImage)
        {
            var url = nm.ToAbsoluteUri($"api/files/thumb/{node.Id}");
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
        */ 

        return "";
    }



    #endregion



    #region "-- Annotations --"
    public async Task ShowNewAnnotationDialog()
    {
        var parameters = new DialogParameters<NodeAddAnnotationDialog> { { x => x.Model, Model } };
        var dialog = await srvDialog.ShowAsync<NodeAddAnnotationDialog>("New Annotation", parameters);
        var result = await dialog.Result;
        if (!result.Canceled && result.Data != null)
        {
            var text = result.Data.ToString().Trim();
            var resp = await CreateAnnotation(text);
            if (!resp.Success)
            {
                snackbar.Add(resp.Message, Severity.Error);
            }
            else
            {
                await ReloadNodeAsync();
            }
        }
    }


    public async Task<BaseResponse> CreateAnnotation(string Text)
    {
        if( _node != null && cAppState.SessionUser.UserId > 0)
        {
            return await srvData.Send(new CreateNodeAnnotationCommand(NodeId: _node.Id, UserId: cAppState.SessionUser.UserId, Text: Text));
        }
        return new NodeCommandResponse(false, "no data or user");
    }

    public bool CanDeleteAnnotation => true;

    public async Task DeleteAnnotation(AnnotationModel item)
    {
        if (Model != null && CanDeleteAnnotation)
        {
            srvData.Send(new DeleteNodeAnnotationCommand(AnnotationId: item.Id));
        }
    }


    #endregion


    #region "-- Attachments --"
    #endregion
}



public enum eNodeCommands
{
    Edit,
    Save,
    Cancel,
    Delete,
    AddAttachment,
    AddAnnotation
}
