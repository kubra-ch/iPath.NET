using iPath.Application.Features;
using iPath.Data.Entities;

namespace iPath.UI.ViewModels.Nodes;

public interface INodeViewModel
{
    Task LoadNode(int NodeId);

    NodeModel Model { get; }

    // Gallery Browsing
    void SelectChild(int ChildId);
    NodeModel ActiveChild { get; set; }
    void PreviousImage();
    void NextImage();


    // Display
    string SubTitleText { get; }
    bool CollapseDescritpion { get; set; }
    string Message { get; }


    // Commands
    Task<UploadNodeFileResponse> UploadFileAsync(int UserId, string Filename, FileInfo localFile);

    Task<AddNodeAnnotationResponse> AddAnnotationAsync(int UserId, string text);

    Task<NodeCommandRespone> UpdateNodeAsync();


    Task<NodeCommandRespone> DeleteNodeAsync(int NodeId);
    Task<NodeCommandRespone> DeleteNodesAsync(List<int> NodeIds);

    Task<NodeCommandRespone> UpdateNodeVisibilityAsync(int NodeId, eNodeVisibility newValue);
    Task<NodeCommandRespone> UpdateNodesVisibilityAsync(List<int> NodeIds, eNodeVisibility newValue);
}
