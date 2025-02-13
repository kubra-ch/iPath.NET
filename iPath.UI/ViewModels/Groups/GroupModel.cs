using iPath.Application.Features;
using iPath.Data.Entities;

namespace iPath.UI.ViewModels;

public class GroupModel
{
    public GroupModel()
    {
    }

    public GroupModel(GroupDto dto)
    {
        Id = dto.Id;
        Name = dto.Name;
        Purpose = dto.Purpose;
        GroupType = dto.GroupType;
        Visibility = dto.Visibility;
        Owner = dto.Owner;
        Community = dto.Community;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string? Purpose { get; set; }
    public eGroupType GroupType { get; set; }
    public eGroupVisibility Visibility { get; set; }
    public UserListDto? Owner { get; set; }
    public CommunityDto Community { get; set; }  


    public GroupDto ToDto()
    {
        return new GroupDto(Id: Id, Name: Name, Purpose: Purpose, 
            GroupType: GroupType, Visibility: Visibility,
            Owner: Owner, Community: Community);
    } 

}
