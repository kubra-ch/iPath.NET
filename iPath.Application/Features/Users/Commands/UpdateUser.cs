using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class UpdateUserCommand : IRequest<UserCommandResponse>
{
    public int Id { get; init; }
    public string? Firstname { get; set; }
    public string? Familyname { get; set; }
    public string? Country { get; set; }

    public string? Specialisation { get; set; }
}



public class UpdateUserCommandHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<UpdateUserCommand, UserCommandResponse>
{
    public async Task<UserCommandResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();

        // get the User from DB
        var item = await ctx.Users.FindAsync(request.Id);
        if (item == null) return new UserCommandResponse(false, Message: $"User #{request.Id} not found");

        if( request.Firstname != null) item.Firstname = request.Firstname;
        if (request.Familyname != null) item.Familyname = request.Familyname;
        if (request.Country != null) item.Country = request.Country;
        if (request.Specialisation != null) item.Specialisation = request.Specialisation;
        item.ModifiedOn = DateTime.UtcNow;

        ctx.Users.Update(item);
        await ctx.SaveChangesAsync();
        return new UserCommandResponse(true, Data: item);
    }
}