using iPath.Data.Entities;

namespace iPath.Application.Events;

public class UserProfileUpdatedEvent : NotificationEvent
{
    public UserProfileUpdatedEvent()
    {
    }

    public static UserProfileUpdatedEvent CreateEvent(User u)
    {
        return new UserProfileUpdatedEvent
        {
            UserId = u.Id,
            Date = DateTime.UtcNow
        };
    }
}
