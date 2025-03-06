using iPath.Application.Events;
using iPath.Data;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;

public record UpdateUserProfileCommand(int UserId, UserProfile Profile) : IRequest<UserCommandResponse>;


public class UpdateUserCommandHandler(IDbContextFactory<NewDB> dbFactory, IMediator mediator)
    : IRequestHandler<UpdateUserProfileCommand, UserCommandResponse>
{
    public async Task<UserCommandResponse> Handle(UpdateUserProfileCommand request, CancellationToken ctk)
    {
        using var ctx = await dbFactory.CreateDbContextAsync(ctk);

        // get the User from DB
        var item = await ctx.Users.FindAsync(request.UserId, ctk);
        if (item == null) return new UserCommandResponse(false, Message: $"User #{request.UserId} not found");

        item.Profile = request.Profile;
        item.ModifiedOn = DateTime.UtcNow;

        ctx.Users.Update(item);
        await ctx.SaveChangesAsync(ctk);

        // publish event
        await mediator.Publish(UserProfileUpdatedEvent.CreateEvent(item), ctk);

        return new UserCommandResponse(true, Data: item);
    }
}