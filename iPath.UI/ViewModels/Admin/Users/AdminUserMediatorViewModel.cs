using iPath.Application.Features;
using iPath.Application.Querying;
using iPath.Data.Entities;
using MediatR;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Admin.Users;

public class AdminUserMediatorViewModel(IMediator mediator) : IAdminUserViewModel
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

        var result = await mediator.Send(request);
        return result.Items;
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

            var result = await mediator.Send(request);

            return GridItemsProviderResult.From(
                items: result.Items,
                totalItemCount: result.TotalItemsCount
                );
        };
    }

    private GridItemsProvider<User> _GridDataProvider = default!;
    public GridItemsProvider<User> GridDataProvider => _GridDataProvider;



    public async Task<User> SelectUserId(int Id)
    {
        _selectedUser = await mediator.Send(new GetUserQuery { Id = Id });
        return _selectedUser;
    }

    private User _selectedUser = null;
    public User SelectedUser => _selectedUser;



    public async Task<CreateUserResponse> CreateUserAsync(string username, string email, string password)
    {
        return await mediator.Send(new CreateUserCommand(Username: username, Email: email, Password: password));
    }

    public Task<UpdateUserResponse> UpdateUserAsync(User item)
    {
        var request = new UpdateUserCommand()
        {
            Item = item
        };
        var response = mediator.Send(request);
        return response;
    }

    public async Task<UpdateUserResponse> UpdateUserNameAsync(string username)
    {
        return await mediator.Send(new UpdateUserNameCommand(SelectedUser.Id, username));
    }

    public async Task<UpdateUserResponse> UpdateUserEmailAsync(string email)
    {
        return await mediator.Send(new UpdateUserEmailCommand(SelectedUser.Id, email));
    }

    public async Task<UpdateUserResponse> UpdateUserPasswordAsync(string Password, bool IsActive)
    {
        return await mediator.Send(new UpdateUserPasswordCommand(UserId: SelectedUser.Id, newPassword: Password, IsActive: IsActive));
    }
}
