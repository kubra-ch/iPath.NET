﻿using iPath.Application.Features;
using iPath.Application.Querying;
using iPath.UI.Areas.DataAccess;
using Microsoft.FluentUI.AspNetCore.Components;

namespace iPath.UI.ViewModels.Admin.Users;

public class AdminUserViewModel(IDataAccess srvData) : IAdminUserViewModel
{
    public async Task<List<UserListDto>> FindUsersAsync(string term)
    {
        var request = new GetUserListDtoQuery(); ;

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

            // sorting
            request.SortDefinitions = new();
            var sort = req.GetSortByProperties();
            if (sort != null && sort.Any())
            {
                foreach (var p in sort)
                {
                    var sd = new SortDefinition { SortColumn = p.PropertyName, SortAscending = (p.Direction == SortDirection.Ascending) };
                    request.SortDefinitions.Add(sd);
                }
            }
            else
            {
                request.SortDefinitions.Add(new SortDefinition { SortColumn = "CreatedOn", SortAscending = false });
            }

            var response = await srvData.Send(request);
            if (!response.Success) throw new Exception(response.Message);

            return GridItemsProviderResult.From(
                items: response.Data.Items,
                totalItemCount: response.Data.TotalItemsCount
                );
        };
    }

    private GridItemsProvider<UserDto> _GridDataProvider = default!;
    public GridItemsProvider<UserDto> GridDataProvider => _GridDataProvider;



    public async Task<UserModel> SelectUserId(int Id)
    {
        _selectedUser = new UserModel((await srvData.Send(new GetUserQuery(Id: Id))).Data);
        return _selectedUser;
    }

    private UserModel _selectedUser = null;
    public UserModel SelectedUser => _selectedUser;



    public async Task<Application.Features.UserCommandResponse> CreateUserAsync(string username, string email, string password)
    {
        return await srvData.Send(new CreateUserCommand(Username: username, Email: email, Password: password));
    }


    public Task<UserCommandResponse> UpdateUserAsync(UserModel item)
    {
        var request = new UpdateUserCommand()
        {
            Id = item.Id,
            Familyname = item.Familyname,
            Firstname = item.Firstname,
            Specialisation = item.Specialisation,
            Country = item.Country,
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
