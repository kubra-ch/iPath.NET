using iPath.Application.Configuration;
using iPath.Application.Services;
using iPath.Data.Database;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace iPath.Application.Features;
public record UploadNodeFileCommand(int NodeId, int UserId, string filename, string localFilePath) : IRequest<UploadNodeFileResponse>
{
}

public record UploadNodeFileResponse(bool Success, Node? childNode= null!, string? Message = default!);

public class UploadNodeFileCommandHandler(IDbContextFactory<IPathDbContext> dbFactory, 
    IOptions<iPathConfig> opts,
    ThumbImageService thumbService,
    ILogger<UploadNodeFileCommandHandler> logger) 
    : IRequestHandler<UploadNodeFileCommand, UploadNodeFileResponse>
{
    public async Task<UploadNodeFileResponse> Handle(UploadNodeFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var fi = new System.IO.FileInfo(request.localFilePath);
            if (!fi.Exists) return new UploadNodeFileResponse(false, Message: "file not found");

            // get parent node
           using var ctx = await dbFactory.CreateDbContextAsync();
            var parent = await ctx.Nodes
                .Include(n => n.ChildNodes)
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Id == request.NodeId);

            if (parent is null) return new UploadNodeFileResponse(false, Message: "parent not found");

            // create entity
            var newNode = new Node();
            newNode.ParentNodeId = parent.Id;
            newNode.TopNodeId = parent.TopNodeId.HasValue ? parent.TopNodeId : parent.Id;
            newNode.NodeType = NodeType.Image;
            newNode.CreateOn = DateTime.Now;
            newNode.OwnerId = request.UserId;
            newNode.SortNr = parent.ChildNodes.Max(n => n.SortNr) + 1;

            // set the status to none and visibility to tmp. Files are uploaded immediately. 
            // When upload is "saved", status must be updated to 
            newNode.Status = eNodeStatus.None;
            newNode.Visibility = eNodeVisibility.Draft;

            newNode.File = new()
            {
                Filename = request.filename,
                MimeType = GetMimeType(request.filename),
                IsImage = IsImage(request.filename)
            };

            await ctx.Nodes.AddAsync(newNode);
            await ctx.SaveChangesAsync();

            if( newNode.Id>0)
            {
                // move file
                var fn = System.IO.Path.Combine(opts.Value.DataPath, newNode.Id.ToString());
                logger.LogInformation("file upload, copy to: " + fn);
                fi.CopyTo(fn);
                // we only copy the file here .. deleting the original must be done by the uploader

                if (newNode.File.IsImage)
                {
                    thumbService.UpdateNode(newNode.File, fn);
                    await ctx.SaveChangesAsync();
                }
            }

            return new UploadNodeFileResponse(true, childNode: newNode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return new UploadNodeFileResponse(false, Message: ex.InnerException is null ? ex.Message : ex.InnerException.Message);
        }
    }



    private static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPEG", ".JPE", ".BMP", ".GIF", ".PNG" };

    private bool IsImage(string Filename)
    {
        try
        {
            var fi = new System.IO.FileInfo(Filename);
            return (ImageExtensions.Contains(fi.Extension.ToUpper()));
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        }
        return false;
    }

    private string GetMimeType(string Filename)
    {
        if (MimeTypes.TryGetMimeType(Filename, out var mimeType))
        {
            return mimeType;
        }
        return "application/octet-stream";
    }

}
