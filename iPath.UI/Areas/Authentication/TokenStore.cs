using Blazored.LocalStorage;
using iPath.Data.Configuration;
using Microsoft.Extensions.Options;

namespace iPath.UI.Areas.Authentication;

public interface ITokenStore
{
    public Task<string?> GetTokenAsync(bool RefreshToken = false);
    public Task SetTokenAsync(string token, bool RefreshToken = false);
    Task DeleteTokenAsync(bool RefreshToken = false);

    string? Token { get; }
}


public class LocalTokenStore : ITokenStore
{
    private readonly ILocalStorageService srv;

    private string Key(bool RefreshToken) => _installationName + "_jwt_" + (RefreshToken ? "refresh" : "access");

    private TokenCache _cache;
    private string _installationName = "ipath";
    public string? Token => _cache?.Token;


    public LocalTokenStore(ILocalStorageService localStore, TokenCache cache, IOptions<iPathConfig> opts)
    {
        srv = localStore;
        _cache = cache;
        _installationName = opts.Value.InstallationName ?? "ipath";
    }


    public async Task DeleteTokenAsync(bool RefreshToken = false)
    {
        _cache.Token = null!;
        await srv.RemoveItemAsync(Key(RefreshToken));
    }

    public async Task<string?> GetTokenAsync(bool RefreshToken = false)
    {
        try
        {
            _cache.Token = await srv.GetItemAsStringAsync(Key(RefreshToken));
            return _cache.Token;
        }
        catch (Exception ex)
        {
        }
        return "";
    }

    public async Task SetTokenAsync(string token, bool RefreshToken = false)
    {
        _cache.Token = token;
        await srv.SetItemAsStringAsync(Key(RefreshToken), token);
    }
}



public class TokenCache
{
    public string Token { get; set; }  
}