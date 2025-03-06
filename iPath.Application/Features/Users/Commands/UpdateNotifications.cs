using iPath.Data;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPath.Application.Features;


public record GroupNotificationDto (int GroupId, eNotification Notifications);


public record UpdateNotificationsCommand(int UserId, GroupNotificationDto[] Notifications) : IRequest<BaseResponse>;


public class UpdateNotificationsCommandHandler(IDbContextFactory<NewDB> fct)
    : IRequestHandler<UpdateNotificationsCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(UpdateNotificationsCommand request, CancellationToken cancellationToken)
    {
        using var ctx = await fct.CreateDbContextAsync();
        var set = ctx.Set<GroupMember>();

        // reload from DB
        var list = await set.Where(m => m.UserId == request.UserId).ToListAsync();

        // remove those set to None
        foreach (var entity in list)
        {
            var dto = request.Notifications.FirstOrDefault(n => n.GroupId == entity.GroupId);
            eNotification n = dto is null ? eNotification.None : dto.Notifications;
            if( entity.Notifications != n)
            {
                entity.Notifications = n;
                ctx.Update(entity);
            }
        }

        await ctx.SaveChangesAsync();

        return new BaseResponse(true, "");
    }
}