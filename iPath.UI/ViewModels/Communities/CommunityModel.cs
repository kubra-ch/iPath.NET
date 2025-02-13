using iPath.Application.Features;

namespace iPath.UI.ViewModels;

public class CommunityModel 
{
    public CommunityModel()
    {
    }

    public CommunityModel(CommunityDto community, List<GroupDto> groups = null!)
    {
        Id = community.Id;
        Name = community.Name;
        Description = community.Description;
        Owner = community.Owner;
        Groups = groups != null ? groups : new List<GroupDto>();
    }


    public int? Id { get; private set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public UserListDto? Owner { get; set; }

    public List<GroupDto> Groups { get; set; } = new();
}
