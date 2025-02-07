using iPath.Application.Features;
using iPath.Application.Querying;
using iPath.Data.Entities;
using iPath.UI.ViewModels.DataService;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Admin.Users;

public class AdminUserViewModel(IDataAccess srvData) : IAdminUserViewModel
{
    public async Task<List<User>> FindUsersAsync(string term)
    {
        var request = new GetUserListQuery(); ;

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            request.Filter ??= new();
            request.Filter.AddFilter("Username", SearchTerm);
        }

        request.StartIndex = 0;
        request.Count = 100;
        request.SortDefinitions ??= new();
        request.SortDefinitions.Add(new SortDefinition { SortColumn = "Username" });

        var response = await srvData.Send(request);
        if( !response.Success ) throw new Exception(response.Message);
        return response.Data.Items;
    }


    public string SearchTerm { get; set; } = default!;
    public bool ActiveOnly { get; set; } = true;


    public async Task ExecuteSearchAsync()
    {
        var request = new GetUserListQuery { IsActive = this.ActiveOnly };

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            request.Filter ??= new();
            request.Filter.AddFilter("Username", SearchTerm);
        }

        // get datae
        _GridDataProvider = async req =>
        {
            request.StartIndex = req.StartIndex;
            request.Count = req.Count;
            request.SortDefinitions ??= new();
            request.SortDefinitions.Add(new SortDefinition { SortColumn = "Username" });

            var response = await srvData.Send(request);
            if (!response.Success) throw new Exception(response.Message);

            return GridItemsProviderResult.From(
                items: response.Data.Items,
                totalItemCount: response.Data.TotalItemsCount
                );
        };
    }

    private GridItemsProvider<User> _GridDataProvider = default!;
    public GridItemsProvider<User> GridDataProvider => _GridDataProvider;



    public async Task<User> SelectUserId(int Id)
    {
        _selectedUser = (await srvData.Send(new GetUserQuery(Id: Id))).Data;
        return _selectedUser;
    }

    private User _selectedUser = null;
    public User SelectedUser => _selectedUser;



    public async Task<Application.Features.UserCommandResponse> CreateUserAsync(string username, string email, string password)
    {
        return await srvData.Send(new CreateUserCommand(Username: username, Email: email, Password: password));
    }


    public Task<UserCommandResponse> UpdateUserAsync(User item)
    {
        var request = new UpdateUserCommand()
        {
            Item = item
        };
        var response = srvData.Send(request);
        return response;
    }

    public async Task<UserCommandResponse> UpdateUserNameAsync(string username)
    {
        return await srvData.Send(new UpdateUserNameCommand(SelectedUser.Id, username));
    }

    public async Task<UserCommandResponse> UpdateUserEmailAsync(string email)
    {
        return await srvData.Send(new UpdateUserEmailCommand(SelectedUser.Id, email));
    }

    public async Task<UserCommandResponse> UpdateUserPasswordAsync(string Password, bool IsActive)
    {
        return await srvData.Send(new UpdateUserPasswordCommand(UserId: SelectedUser.Id, newPassword: Password, IsActive: IsActive));
    }
}
