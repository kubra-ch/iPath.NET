using Microsoft.AspNetCore.Components;

namespace iPath.Net.UI.Components.Shared;

public class DisposableComponentBase : ComponentBase, IDisposable
{
    private CancellationTokenSource? cancellationTokenSource;

    protected CancellationToken CancellationToken => (cancellationTokenSource ??= new()).Token;

    public virtual void Dispose()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }
    }
}
