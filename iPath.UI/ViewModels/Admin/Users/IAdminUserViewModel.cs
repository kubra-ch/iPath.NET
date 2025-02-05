using iPath.Application.Features;
using iPath.Data.Entities;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Admin.Users;

public interface IAdminUserViewModel
{
    GridItemsProvider<User> GridDataProvider { get; }
    Task<List<User>> FindUsersAsync(string term);

    string SearchTerm { get; set; }
    Task ExecuteSearchAsync();

    Task<User> SelectUserId(int Id);
    User SelectedUser { get; }

    Task<CreateUserResponse> CreateUserAsync(string Username, string Email, string Password);
    Task<UpdateUserResponse> UpdateUserAsync(User item);

    Task<UpdateUserResponse> UpdateUserNameAsync(string USername);
    Task<UpdateUserResponse> UpdateUserEmailAsync(string Email);
}
