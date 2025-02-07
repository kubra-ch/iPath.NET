using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPath.Application.Features;


public record UpdateAnnotationCommand(int Id, int? UserId = null!, string? Text = null!, eAnnotationVisibility? Visibility = null!)
    : IRequest<AnnotationCommandResponse>
{
}


public class UpdateAnnotationCommandHandler(IDbContextFactory<IPathDbContext> dbFactory)
    : IRequestHandler<UpdateAnnotationCommand, AnnotationCommandResponse>
{
    public async Task<AnnotationCommandResponse> Handle(UpdateAnnotationCommand request, CancellationToken cancellationToken)
    {
        // get the node from the DB
        using var ctx = await dbFactory.CreateDbContextAsync();
        var anno = await ctx.NodeAnnotations.FindAsync(request.Id);

        if (anno is null) return new AnnotationCommandResponse(false, Message: $"Annotation #{request.Id} not found");

        // update Text?
        if (request.Text != null)
        {
            anno.Text = request.Text;
        }

        // update visibility
        if (request.Visibility != null)
        {
            anno.Visibility = request.Visibility;
        }

        // Update Owner?
        if (request.UserId.HasValue)
        {
            var user = await ctx.Users.FindAsync(request.UserId);
            if (user != null) anno.Owner = user;
        }

        // remove empty drafts or deleted from db
        if (string.IsNullOrWhiteSpace(anno.Text))
        {
            if (anno.Visibility == eAnnotationVisibility.Deleted || anno.Visibility == eAnnotationVisibility.Draft)
            {
                ctx.NodeAnnotations.Remove(anno);
                await ctx.SaveChangesAsync();
                return new AnnotationCommandResponse(true, Message: "Empty Annotaiton deleted");
            }
        }

        // Save Changed to DB
        anno.ModifiedOn = DateTime.UtcNow;
        await ctx.SaveChangesAsync();

        return new AnnotationCommandResponse(true, Data: anno);
    }
}
