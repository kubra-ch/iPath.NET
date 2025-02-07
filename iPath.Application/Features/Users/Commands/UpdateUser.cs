using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class UpdateUserCommand : IRequest<UserCommandResponse>
{
    public User Item { get; set; }
}



public class UpdateUserCommandHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<UpdateUserCommand, UserCommandResponse>
{
    public async Task<UserCommandResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();

        // get the User from DB
        var item = await ctx.Users.FindAsync(request.Item.Id);

        item.Firstname = request.Item.Firstname;
        item.Familyname = request.Item.Familyname;
        item.Country = request.Item.Country;
        item.Specialisation = request.Item.Specialisation;
        item.ModifiedOn = DateTime.UtcNow;

        ctx.Users.Update(item);
        await ctx.SaveChangesAsync();
        return new UserCommandResponse(true, Data: item);
    }
}