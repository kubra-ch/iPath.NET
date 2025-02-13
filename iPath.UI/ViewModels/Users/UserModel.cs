using iPath.Application.Features;
using iPath.Data.Entities;


namespace iPath.UI.ViewModels;

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
        
        Username = pDto.Username;
        Email = pDto.Email;
        IsActive = pDto.IsActive;
        IsSysAdmin = pDto.IsSysAdmin;

        _OrigUsername = pDto.Username;
        _OrigEmail = pDto.Email;
        _OrigActive = pDto.IsActive;
        _OrigSysAdmin = pDto.IsSysAdmin;
    }

    private string _OrigUsername; 
    private string _OrigEmail;
    private bool _OrigActive;
    private bool _OrigSysAdmin;


    public int Id => dto.Id;
    public string Username { get; set; }
    public string Email { get; set; }

    public string ImageBase64 { get; set; }


    public string Familyname { get; set; }
    public string Firstname { get; set; }
    public string Specialisation { get; set; }
    public string Country { get; set; }

    public bool IsActive { get; set; }
    public bool IsSysAdmin { get; set; }


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
