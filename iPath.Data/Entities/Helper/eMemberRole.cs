namespace iPath.Data.Entities;

[Flags]
public enum eMemberRole
{
    None = 0,
    Admin = 1,
    Moderator = 2,
    User = 4,
    Guest = 8
}