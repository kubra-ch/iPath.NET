namespace iPath.Application.Services;


public interface ISessionStateService
{ 
    int? SessionUserId { get; set; }
}

public class SessionStateService : ISessionStateService
{
    public int? SessionUserId { get; set; }
}
