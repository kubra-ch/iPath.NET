using iPath.Data.Entities;
using iPath.UI.Components.Users.Dialogs;
using MudBlazor;

namespace iPath.UI.Components.Shared;

public static class DialogExtensions
{
    public static async Task ShowProfile(this IDialogService srvDialog, UserProfile profile)
    {
        if( profile == null || srvDialog == null ) return;

        var parameters = new DialogParameters<UserProfileDialog> { { x => x.Profile, profile } };
        var options = new DialogOptions { CloseOnEscapeKey = true };
        var dialog = await srvDialog.ShowAsync<UserProfileDialog>("User Profile", parameters: parameters, options: options);
        var result = await dialog.Result;
    }
}
