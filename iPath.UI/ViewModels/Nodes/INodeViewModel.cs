using iPath.Application.Features;
using iPath.Data.Entities;
using iPath.UI.Areas.DraftStorage;
using iPath.UI.ViewModels.Drafts;

namespace iPath.UI.ViewModels.Nodes;

public interface INodeViewModel
{
    void ResetData();
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
    Task<NodeCommandRespone> UpdateNodeAsync();

    Task<NodeCommandRespone> DeleteNodeAsync(int NodeId);
    Task<NodeCommandRespone> DeleteNodesAsync(List<int> NodeIds);

    Task<NodeCommandRespone> UpdateNodeVisibilityAsync(int NodeId, eNodeVisibility newValue);
    Task<NodeCommandRespone> UpdateNodesVisibilityAsync(List<int> NodeIds, eNodeVisibility newValue);

    Task<NodeCommandRespone> UpdateSortNumerbs(List<(int NodeId, int SortNr)> newOrder);

    // Attachments
    Task<NodeCommandRespone> UploadFileAsync(int UserId, string Filename, FileInfo localFile);
    string ThumbUrl(NodeModel node);
    string FileUrl(NodeModel node);


    // Annotations
    Task<AnnotationCommandResponse> CreateAnnotationAsync(int userId, string? text = null!);
    Task<AnnotationCommandResponse> CreateAnnotationAsync(CreateAnnotationDraft draft);
    Task<AnnotationCommandResponse> UpdateAnnotationAsync(AnnotationModel model, string? text = null!, eAnnotationVisibility? visibility = null!, int? userId = null!);
    Task<AnnotationCommandResponse> DeleteAnnotationAsync(AnnotationModel model);

    bool AnnotationIsVisible(AnnotationModel model);



    Task<CreateAnnotationDraft> GetAnnotationDraft(bool autoCreate);
    IDraftStore DraftStore { get; }

}
