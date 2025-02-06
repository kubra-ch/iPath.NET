using iPath.Data.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iPath.Application.Features;

public record UpdateUserEmailCommand(int UserId, [EmailAddress] string Email)
    : IRequest<UpdateUserResponse> { }


public class UpdateUserEmailCommandHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<UpdateUserEmailCommand, UpdateUserResponse>
{
    public async Task<UpdateUserResponse> Handle(UpdateUserEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
           using var ctx = await dbFactory.CreateDbContextAsync();

            // trim
            var email = request.Email.Trim();

            // find other user with different id but same new name
            var exists = await ctx.Users.AnyAsync(u => u.EmailInvariant == email.ToLowerInvariant() && u.Id != request.UserId);
            if( exists)
                return new UpdateUserResponse(false, Message: $"Another User with email {email} already exists"); 


            // get the User from DB
            var item = await ctx.Users.FindAsync(request.UserId);

            // update properties
            item.Email = email;
            item.EmailInvariant = email.ToLowerInvariant();
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