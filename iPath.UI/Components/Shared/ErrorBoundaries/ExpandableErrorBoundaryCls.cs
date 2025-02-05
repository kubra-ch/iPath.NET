using Microsoft.AspNetCore.Components.Web;

namespace iPath.UI.Components.Shared.ErrorBoundaries;

public class ExpandableErrorBoundaryCls : ErrorBoundary
{
    public string ErrorMessage => CurrentException?.Message;
    public string StackTrace => CurrentException?.StackTrace;
    public bool HasError => CurrentException != null;
}