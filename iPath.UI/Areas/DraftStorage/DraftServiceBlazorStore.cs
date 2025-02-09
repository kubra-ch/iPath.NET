using Blazored.LocalStorage;
using iPath.UI.ViewModels.Drafts;

namespace iPath.UI.Areas.DraftStorage;

public class DraftServiceBlazorStore(ILocalStorageService storage) : IDraftStore
{
    public async Task CleanDraftsAsync(DateTime before)
    {
        foreach( var k in await storage.KeysAsync() )
        {
            if( k.StartsWith("draft-"))
            {
                await storage.RemoveItemAsync(k);
            }
        }
    }

    public async Task<T> GetDraft<T>(string key) where T : IDraft
    {
        var item = await storage.GetItemAsync<T>(key);
        return (T)item;
    }

    public async Task RemoveAsync(string key)
    {
        await storage.RemoveItemAsync(key);
    }

    public async Task SetDraft<T>(T draft) where T : IDraft
    {
        await storage.SetItemAsync<T>(draft.DraftId, draft);
    }

    async Task<List<string>> IDraftStore.KeysAsync()
    {
        var ret = new List<string>();
        foreach (var k in await storage.KeysAsync())
        {
            if (k.StartsWith("draft-"))
            {
                ret.Add(k);
            }
        }
        return ret;
    }
}
