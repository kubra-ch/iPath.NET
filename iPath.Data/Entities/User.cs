using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace iPath.Data.Entities;

public class User : BaseEntityWithDeleteFlag
{
    public string Username { get; set; }
    public string UsernameInvariant { get; set; }
    public string Email { get; set; }
    public string EmailInvariant { get; set; }

    public string PasswordHash { get; set; }
    public string? iPath2Password { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }


    public ICollection<UserRole> Roles { get; set; } = [];
    public ICollection<GroupMember> GroupMembership { get; set; } = [];
    public ICollection<CommunityMember> CommunityMembership { get; set; } = [];

    public UserProfile? Profile { get; set; }

    public ICollection<FileUpload> Uploads { get; set; }
    public ICollection<NodeLastVisit> NodeVisitis { get; set; }
    public ICollection<UserNotification> Notifications { get; set; }
}


public class UserProfile
{
    public int UserId { get; set; }

    [MaxLength(50), RegularExpression("^[A-Za-z][A-Za-z0-9_]{3,50}$")]
    public string Username { get; set; }

    [JsonPropertyName("familyname"), MaxLength(50)]
    public string FamilyName { get; set; }

    [JsonPropertyName("firstname"), MaxLength(50)]
    public string FirstName { get; set; }

    [JsonPropertyName("initials"), MaxLength(3)]
    public string Initials { get; set; }


    [JsonPropertyName("specialisation")]
    public string Specialisation { get; set; }

    public ICollection<ContactDetails> ContactDetails { get; set; } = [];


    public static UserProfile AnonymousProfile()
    {
        return new UserProfile
        {
            Username = "anonymous",
            ContactDetails = new ContactDetails[] { new ContactDetails() { IsMainContact = true, Address = new() } }
        };
    }

    public static UserProfile EmptyProfile()
    {
        return new UserProfile
        {
            ContactDetails = new ContactDetails[] { new ContactDetails() { IsMainContact = true, Address = new() } }
        };
    }
}


public class ContactDetails
{
    public bool IsMainContact { get; set; }
    public string? PhoneNr { get; set; }
    public string? Email { get; set; }
    public string? Organisation { get; set; }
    public Address? Address { get; set; }
}


public class Address
{
    public string? Street { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}



public class UserRole : BaseEntity
{
    public string Name { get; set; }

    public static UserRole Admin => new UserRole { Id = 1, Name = "Admin" };
    public static UserRole Moderator => new UserRole { Id = 2, Name = "Moderator" };
}