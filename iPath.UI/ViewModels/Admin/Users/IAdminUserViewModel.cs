using iPath.Application.Features;
using iPath.Data.Entities;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Admin.Users;

public interface IAdminUserViewModel
{
    GridItemsProvider<User> GridDataProvider { get; }
    Task<List<User>> FindUsersAsync(string term);

    string SearchTerm { get; set; }
    bool ActiveOnly { get; set; }
    Task ExecuteSearchAsync();

    Task<User> SelectUserId(int Id);
    User SelectedUser { get; }

    Task<UserCommandResponse> CreateUserAsync(string Username, string Email, string Password);
    Task<UserCommandResponse> UpdateUserAsync(User item);

    Task<UserCommandResponse> UpdateUserNameAsync(string USername);
    Task<UserCommandResponse> UpdateUserEmailAsync(string Email);

    Task<UserCommandResponse> UpdateUserPasswordAsync(string Password, bool IsActive);
}
