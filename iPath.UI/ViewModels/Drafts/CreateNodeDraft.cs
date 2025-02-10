using iPath.Application.Features;
using Microsoft.IdentityModel.Abstractions;

namespace iPath.UI.ViewModels.Drafts;

public class CreateNodeDraft : IDraft
{
    public static CreateNodeDraft ForParentNode(int parentNodeId)
    {
        return new CreateNodeDraft
        {
            DraftId = NodeKeyForParent(parentNodeId),
            ParentNodeId = parentNodeId,            
            CreatedOn = DateTime.UtcNow
        };
    }
    public static CreateNodeDraft ForGroup(int groupId)
    {
        return new CreateNodeDraft
        {
            DraftId = NodeKeyForGroup(groupId),
            GroupId = groupId,
            CreatedOn = DateTime.UtcNow
        };
    }

    public static string NodeKeyForParent(int parentNodeId) => $"{CacheKeySeed}-parent-{parentNodeId}";
    public static string NodeKeyForGroup(int groupId) => $"{CacheKeySeed}-group-{groupId}";

    public string DraftId { get; set; }
    public int? GroupId { get; private set; }
    public int? ParentNodeId { get; private set; }

    public DateTime CreatedOn { get; set; }

    public UserDto User { get; set; }

    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Description { get; set; }

    public static string CacheKeySeed => "draft-node" ;
}