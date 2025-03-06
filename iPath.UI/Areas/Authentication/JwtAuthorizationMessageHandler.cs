using System.Net.Http.Headers;

namespace iPath.UI.Areas.Authentication;

public class JwtAuthorizationMessageHandler(TokenCache cache) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tk = cache?.Token;
        if (!string.IsNullOrEmpty(tk))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", tk);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}