using iPath.Application.Configuration;
using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace iPath.Application.Features;


public record CreateAnnotationCommand(int NodeId, int UserId, string? text = null!) : IRequest<AnnotationCommandResponse>
{
}


public class CreateAnnotationCommandHadnler(IDbContextFactory<IPathDbContext> dbFactory, IOptions<iPathConfig> opts)
    : IRequestHandler<CreateAnnotationCommand, AnnotationCommandResponse>
{
    public async Task<AnnotationCommandResponse> Handle(CreateAnnotationCommand request, CancellationToken cancellationToken)
    {
        // get parent node
        using var ctx = await dbFactory.CreateDbContextAsync();
        var parent = await ctx.Nodes
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == request.NodeId);

        if (parent is null) return new AnnotationCommandResponse(false, Message: "parent not found");

        var user = await ctx.Users.FindAsync(request.UserId);
        if (user is null) return new AnnotationCommandResponse(false, Message: "invalid user id");

        // create an Annotation entity in status draft
        var anno = new Annotation();
        anno.Visibility = eAnnotationVisibility.Draft;
        anno.Text = request.text;
        anno.CreatedOn = DateTime.UtcNow;
        anno.NodeId = request.NodeId;
        anno.Owner = user;

        await ctx.NodeAnnotations.AddAsync(anno);
        await ctx.SaveChangesAsync();

        return new AnnotationCommandResponse(true, Data: anno);
    }
}
