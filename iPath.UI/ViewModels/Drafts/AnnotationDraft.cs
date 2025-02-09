using iPath.Application.Features;
using Microsoft.IdentityModel.Abstractions;

namespace iPath.UI.ViewModels.Drafts;

public class AnnotationDraft : IDraft
{
    public static AnnotationDraft ForNode(int NodeId)
    {
        return new AnnotationDraft
        {
            DraftId = NodeKey(NodeId),
            NodeId = NodeId,
            CreatedOn = DateTime.UtcNow
        };
    }

    public static string NodeKey(int NodeId) => $"{CacheKeySeed}-node-{NodeId}";

    public string DraftId { get; set; }
    public int NodeId { get; private set; }

    public DateTime CreatedOn { get; set; }

    public UserDto User { get; set; }
    public string? Text { get; set; }

    public static string CacheKeySeed => "draft-annotation" ;
}