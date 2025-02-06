using iPath.Application.Configuration;
using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace iPath.Application.Features;


public record CreateAnnotationCommand(int NodeId, int UserId, string? text = null!) : IRequest<UpdateAnnotationResponse>
{
}


public class CreateAnnotationCommandHadnler(IDbContextFactory<IPathDbContext> dbFactory, IOptions<iPathConfig> opts)
    : IRequestHandler<CreateAnnotationCommand, UpdateAnnotationResponse>
{
    public async Task<UpdateAnnotationResponse> Handle(CreateAnnotationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // get parent node
           using var ctx = await dbFactory.CreateDbContextAsync();
            var parent = await ctx.Nodes
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Id == request.NodeId);

            if (parent is null) return new UpdateAnnotationResponse(false, Message: "parent not found");

            var user = await ctx.Users.FindAsync(request.UserId);
            if (user is null) return new UpdateAnnotationResponse(false, Message: "invalid user id");

            // create an Annotation entity in status draft
            var anno = new Annotation();
            anno.Visibility = eAnnotationVisibility.Draft;
            anno.Text = request.text;
            anno.CreatedOn = DateTime.Now;
            anno.NodeId = request.NodeId;
            anno.Owner = user;

            await ctx.NodeAnnotations.AddAsync(anno);
            await ctx.SaveChangesAsync();
                        
            return new UpdateAnnotationResponse(true, item: anno);
        }
        catch (Exception ex)
        {
            return new UpdateAnnotationResponse(false, Message: ex.InnerException is null ? ex.Message : ex.InnerException.Message);
        }
    }
}
