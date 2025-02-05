using Microsoft.JSInterop;

namespace iPath.UI.Componenets.Code;

public class ClipboardService(IJSRuntime jsRuntime)
{
    public ValueTask WriteTextAsync(string text)
    {
        return jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
    }
}