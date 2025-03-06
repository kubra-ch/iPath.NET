using Ardalis.SmartEnum;

namespace iPath.Data.Entities;

public class UserNotification
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }

    public DateTime Date { get; set; }
    public string Message { get; set; }
    public bool Seen { get; set; }
    public eNotificationType Type { get; set; }
}



public sealed class eNotificationType : SmartEnum<eNotificationType>
{
    public static readonly eNotificationType NewAnnotation = new("NewAnnotation", 0);
    public static readonly eNotificationType NewCase = new("NewCase", 1);
    private eNotificationType(string name, int id) : base(name, id) { }
}