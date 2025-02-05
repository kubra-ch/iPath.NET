using System.ComponentModel.DataAnnotations;

namespace iPath.Data.Entities;

public class User : BaseEntity
{
    [Required, MinLength(3), MaxLength(50)]
    public string Username { get; set; }
    public string UsernameInvariant { get; set; }

    [EmailAddress(), MaxLength(100)]
    public string Email { get; set; }
    public string EmailInvariant { get; set; }

    public string PasswordHash { get; set; }

    public bool IsActive { get; set; }
    public bool IsSysAdmin { get; set; }


    [MaxLength(100)]
    public string? Familyname { get; set; }
    [MaxLength(100)]
    public string? Firstname { get; set; }

    [MaxLength(50)]
    public string? Country { get; set; }

    [MaxLength(200)]
    public string? Specialisation { get; set; }

    public string? ImageBase64 { get; set; }

    public ICollection<GroupMember> GroupMembership { get; set; }
}
