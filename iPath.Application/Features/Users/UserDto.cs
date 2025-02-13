using iPath.Data.Entities;

namespace iPath.Application.Features;

public record UserListDto(
    int Id,
    string Username,
    string Email
    );


public record UserDto(
    int Id,
    string Username,
    string Email,
    bool IsActive,
    bool IsSysAdmin,
    string? Familyname,
    string? Firstname,
    string? Country,
    string? Specialisation,
    string? ImageBase64
);

public static class UserExtensions
{
    public static UserListDto ToListDto(this User usr)
    {
        return new UserListDto(Id: usr.Id, Username: usr.Username, Email: usr.Email);
    }

    public static UserDto ToDto(this User usr)
    {
        return new UserDto(
            Id: usr.Id, 
            Username: usr.Username, 
            Email: usr.Email,
            IsActive: usr.IsActive,
            IsSysAdmin: usr.IsSysAdmin,
            Familyname: usr.Familyname,
            Firstname: usr.Firstname,
            Country: usr.Country, 
            Specialisation: usr.Specialisation,
            ImageBase64: usr.ImageBase64
            );
    }
}