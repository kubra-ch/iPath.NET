using iPath.Application.Features;
using iPath.Data.Entities;


namespace iPath.UI.ViewModels.Users;

public class UserModel
{
    private UserDto dto;

    public UserModel(UserDto pDto)
    {
        dto = pDto;
        Familyname = pDto.Familyname;
        Firstname = pDto.Firstname;
        Specialisation = pDto.Specialisation;
        Country = pDto.Country;
    }
    public UserModel(User pEntity)
    {
        dto = pEntity.ToDto();
        Familyname = pEntity.Familyname;
        Firstname = pEntity.Firstname;
        Specialisation = pEntity.Specialisation;
        Country = pEntity.Country;
    }


    public int Id => dto.Id;
    public string Username => dto.Username;
    public string Email => dto.Email;

    public string ImageBase64 => dto.ImageBase64;


    public string Familyname { get; set; }
    public string Firstname { get; set; }
    public string Specialisation { get; set; }
    public string Country { get; set; }


    private string? _initials;
    public string Initials
    {
        get
        {
            if (_initials is null)
            {
                if (!string.IsNullOrEmpty((dto.Firstname))) _initials += dto.Firstname[0];
                if (!string.IsNullOrEmpty((dto.Familyname))) _initials += dto.Familyname[0];
                // fallback to username
                if (string.IsNullOrEmpty(_initials)) _initials = dto.Username[0].ToString();
            }

            return _initials;
        }
    }

    public string Fullname => $"{dto.Firstname} {dto.Familyname}";
}
