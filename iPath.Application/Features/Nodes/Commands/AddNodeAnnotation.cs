using iPath.Application.Configuration;
using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace iPath.Application.Features;
public record AddNodeAnnotationCommand(int NodeId, int UserId, string text) : IRequest<AddNodeAnnotationResponse>
{
}

public record AddNodeAnnotationResponse(bool Success, Annotation? item= null!, string? Message = default!);

public class AddNodeAnnotationCommandHadnler(IPathDbContext ctx, IOptions<iPathConfig> opts) : IRequestHandler<AddNodeAnnotationCommand, AddNodeAnnotationResponse>
{
    public async Task<AddNodeAnnotationResponse> Handle(AddNodeAnnotationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.text)) return new AddNodeAnnotationResponse(false, Message: "no text");

            // get parent node
            var parent = await ctx.Nodes
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Id == request.NodeId);

            if (parent is null) return new AddNodeAnnotationResponse(false, Message: "parent not found");

            var user = await ctx.Users.FindAsync(request.UserId);
            if (user is null) return new AddNodeAnnotationResponse(false, Message: "invalid user id");

            // create entity
            var anno = new Annotation();
            anno.Text = request.text;
            anno.CreatedOn = DateTime.Now;
            anno.NodeId = request.NodeId;
            anno.Owner = user;

            await ctx.NodeAnnotations.AddAsync(anno);
            await ctx.SaveChangesAsync();
                        
            return new AddNodeAnnotationResponse(true, item: anno);
        }
        catch (Exception ex)
        {
            return new AddNodeAnnotationResponse(false, Message: ex.InnerException is null ? ex.Message : ex.InnerException.Message);
        }
    }
}
