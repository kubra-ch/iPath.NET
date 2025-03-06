using iPath.Application.Areas.Authentication;
using iPath.UI.Components.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace iPath.UI.Areas.Authentication;

public class JwtAuthenticationStateProvider(IAuthService srvAuth, ITokenStore srvToken)
    : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal Unauthenticated = new(new ClaimsIdentity());
    private string _error = "";
    public string Error => _error;


    public async Task<bool> Login(LoginRequest login)
    {
        await Logout();
        var resp = await srvAuth.LoginAsync(login.Username, login.Password);
        if (resp.Success)
        {
            // store token for re-use
            await srvToken.SetTokenAsync(resp.AccessToken);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            // keep refreshtoken
            if( login.RememberMe )
            {
                srvToken.SetTokenAsync(resp.RefreshToken, true);
            }

            return true;
        }
        else
        {
            _error = resp.Message;
        }

        return false;
    }


    public async Task Logout()
    {
        await srvToken.DeleteTokenAsync();
        await srvToken.DeleteTokenAsync(true);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = Unauthenticated;

        var token = await srvToken.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            var jwtIdentity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");

            // validate that token is not expired
            var expieryDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(jwtIdentity.Claims.First(x => x.Type == "exp").Value));
            if (expieryDate.UtcDateTime > DateTime.UtcNow)
            {
                // attach token to http request headers
                // http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
                user = new ClaimsPrincipal(jwtIdentity);
            }            
            else
            {
                var rt = await srvToken.GetTokenAsync(true);
                if( !string.IsNullOrEmpty(rt) )
                {
                    Console.WriteLine("JWT has expired => Refresh ... ");
                }

                Console.WriteLine("JWT has expired => Logout");
                await Logout();
            }
        }

        var state = new AuthenticationState(user);
        return state;
    }


    // helper functions to convert token base64 to claims
    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}