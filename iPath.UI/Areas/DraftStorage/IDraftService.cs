using iPath.UI.ViewModels.Drafts;

namespace iPath.UI.Areas.DraftStorage;

public interface IDraftStore
{
    Task<T> GetDraft<T>(string key) where T : IDraft;
    Task SetDraft<T>(T draft) where T : IDraft;
    Task RemoveAsync(string key);

    Task CleanDraftsAsync(DateTime before);
    Task<List<string>> KeysAsync();
}
