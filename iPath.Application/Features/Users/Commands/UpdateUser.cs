using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public class UpdateUserCommand : IRequest<UpdateUserResponse>
{
    public User Item { get; set; }
}


public record UpdateUserResponse(bool Success, User? Item = null!, string? Message = default!);



public class UpdateUserCommandHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
{
    public async Task<UpdateUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
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
            return new UpdateUserResponse(true, item);
        }
        catch(Exception ex)
        {
            return new UpdateUserResponse(false, Message: (ex.InnerException is null ? ex.Message : ex.InnerException.Message));
        }
    }
}