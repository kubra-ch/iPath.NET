using iPath.Data.Entities;

namespace iPath.Application.Features;



public record GroupListDto(int Id, string Name, int? CommunityId, int? NodeCount, int? NewObjCount, int? NewCommentCount);


public record GroupDto(
    int Id,
    string? Name = default,
    string? Purpose = default,
    UserListDto? Owner = null,
    DateTime? CreatedOn = null!,
    eGroupType? GroupType = null!,
    eGroupVisibility? Visibility = null!,
    CommunityDto? Community = null!
    );


public static class GroupExtensions
{
    public static GroupDto ToDto(this Group group)
    {
        return new GroupDto(
            Id: group.Id,
            Name: group.Name,
            Purpose: group.Purpose,
            Owner: group.Owner != null ? group.Owner.ToListDto() : null,
            GroupType: group.GroupType,
            Visibility: group.Visibility,
            Community: group.Community != null ? group.Community.ToDto() : null
        );
    }
}