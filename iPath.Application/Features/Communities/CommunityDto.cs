using iPath.Data.Entities;

namespace iPath.Application.Features;

public record CommunityDto(
    int Id, 
    string? Name=default, 
    string? Description=default, 
    DateTime? CreatedOn=null!, 
    UserListDto? Owner = null, 
    string? BaseUrl=default!
);

public static class CommunityExtension
{
    public static CommunityDto ToDto(this Community community)
    {
        return new CommunityDto(
            Id: community.Id,
            Name: community.Name,
            Description: community.Descritption,
            CreatedOn: community.CreatedOn,
            Owner: (community.Owner != null ? community.Owner.ToListDto() : null!),
            BaseUrl: community.BaseUrl
            );
    }
}