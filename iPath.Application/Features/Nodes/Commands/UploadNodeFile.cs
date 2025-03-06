using iPath.Application.Events;
using iPath.Application.Services;
using iPath.Application.Services.Storage;
using iPath.Data;
using iPath.Data.Configuration;
using iPath.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace iPath.Application.Features;


public record UploadNodeFileCommand(int RootNodeId, int ParentNodeId, int UserId, string filename, string localFilePath) : IRequest<NodeCommandResponse>
{
}


public class UploadNodeFileCommandHandler(IDbContextFactory<NewDB> dbFactory,
    IOptions<iPathConfig> opts,
    IMediator mediator,
    IThumbImageService srvThumb,
    IStorageService srvStorage,
    ILogger<UploadNodeFileCommandHandler> logger)
    : IRequestHandler<UploadNodeFileCommand, NodeCommandResponse>
{
    public async Task<NodeCommandResponse> Handle(UploadNodeFileCommand request, CancellationToken ctk)
    {
        var fi = new System.IO.FileInfo(request.localFilePath);
        if (!fi.Exists) return new NodeCommandResponse(false, Message: "file not found");

        // get root node
        using var ctx = await dbFactory.CreateDbContextAsync(ctk);
        var rootParent = await ctx.Nodes.Include(n => n.ChildNodes).FirstOrDefaultAsync(n => n.Id == request.RootNodeId, ctk);

        if (rootParent is null) return new NodeCommandResponse(false, Message: "parent not found");

        // create entity
        var newNode = new Node();
        newNode.RootNodeId = request.RootNodeId;
        newNode.ParentNodeId = request.ParentNodeId;
        newNode.CreatedOn = DateTime.UtcNow;
        newNode.OwnerId = request.UserId;
        newNode.SortNr = rootParent.ChildNodes.Where(n => n.ParentNodeId == request.ParentNodeId).Max(n => n.SortNr) + 1;
        newNode.SortNr ??= 0;

        // set the status to none and visibility to tmp. Files are uploaded immediately. 
        // When upload is "saved", status must be updated to 

        newNode.File = new()
        {
            Filename = request.filename,
            MimeType = GetMimeType(request.filename),
        };

        // node type
        newNode.NodeType = newNode.File.MimeType.ToLower().StartsWith("image") ? "image" : "file";

        // save node to generate ID
        try
        {
            await ctx.Nodes.AddAsync(newNode);
            await ctx.SaveChangesAsync(ctk);
        }
        catch (Exception ex)
        {
            var msg = ex.InnerException is null ? ex.Message : ex.InnerException.Message;
            Console.WriteLine(msg);
            throw ex;
        }

        // Save the file to local temp folder
        var fn = System.IO.Path.Combine(opts.Value.TempDataPath, newNode.Id.ToString());
        logger.LogInformation("file upload, copy to: " + fn);
        fi.CopyTo(fn);

        // generate thumbnail
        if (newNode.File.MimeType.ToLower().StartsWith("image"))
        {
            newNode.NodeType = "image";
            srvThumb.UpdateNode(newNode.File, fn);
            await ctx.SaveChangesAsync();
        }
        await ctx.SaveChangesAsync();

        // copy to storage
        await srvStorage.PutNodeFileAsync(newNode.Id, ctk);

        // publish domain event
        await mediator.Publish(new NodeUpdatedEvent(rootParent), ctk);

        return new NodeCommandResponse(true, node: newNode);
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
