using iPath.Application.Features;
using iPath.UI.Areas.DataAccess;

namespace iPath.UI.ViewModels.Users;

public class UserProfileViewModel(IDataAccess srvData) : IUserProfileViewModel
{
    private UserModel _model;
    public UserModel Model => _model;

    public async Task LoadUserAsync(int UserId)
    {
        var resp = await srvData.Send(new GetUserQuery(Id: UserId));
        if (resp.Success)
        {
            _model =new UserModel(resp.Data);
        }
        else
        {
            throw new Exception(resp.Message);
        }
    }

    public async Task<UserCommandResponse> SaveModelAsync()
    {
        var cmd = new UpdateUserCommand()
        {
            Id = Model.Id,
            Familyname = Model.Familyname,
            Firstname = Model.Firstname,
            Specialisation = Model.Specialisation,
            Country = Model.Country,
        };

        return await srvData.Send(cmd);
    }
}
