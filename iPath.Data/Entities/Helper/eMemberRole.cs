namespace iPath.Data.Entities;

[Flags]
public enum eMemberRole
{
    None = 0,
    User = 1,
    Inactive = 2,
    Moderator = 4,
    Guest = 8
}