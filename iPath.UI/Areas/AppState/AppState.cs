namespace iPath.UI.Areas.AppState;

public interface IAppState
{
    DateTime LastChanged { get; set; }
    bool DarkTheme { get; set; }
}

public class AppState : IAppState
{
    public DateTime LastChanged { get; set; }
    public bool DarkTheme { get; set; }
}
