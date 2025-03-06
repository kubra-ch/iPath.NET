using iPath.Data;
using iPath.Data.Configuration;
using iPath.Data.Entities;
using iPath.Data.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace iPath.Application.Services.Storage;

public class LocalStorageService(IOptions<iPathConfig> opts, IDbContextFactory<NewDB> dbFactory, ILogger<LocalStorageService> logger)
    : IStorageService
{

    private string _storagePath;
    public string StoragePath 
    {
        get
        {
            if( string.IsNullOrEmpty(_storagePath)) _storagePath = opts.Value.LocalDataPath;
            return _storagePath;
        }
    } 


    public async Task<StorageRepsonse> GetNodeFileAsync(int NodeId, CancellationToken ctk = default!)
    {
        try
        {
            using var ctx = await dbFactory.CreateDbContextAsync(ctk);
            var node = await ctx.Nodes
                .Include(n => n.RootNode)
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Id == NodeId, ctk);

            if (node is null) throw new Exception($"Node {NodeId} not found");

            if (node.RootNode is null || !node.RootNode.GroupId.HasValue)
                return new StorageRepsonse(false, "Root node does not beldong to a group");

            if (string.IsNullOrEmpty(node.StorageId)) throw new Exception("File does not have a StorageId. It has not been previously exported to storage");

            var filePath = Path.Combine(GetNodePath(node.RootNode), node.RootNode.GroupId.ToString() + Path.PathSeparator + node.StorageId);

            if (!File.Exists(filePath)) throw new Exception($"File not found: {filePath}");

            // copy to local file
            var localFile = Path.Combine(opts.Value.TempDataPath, NodeId.ToString());
            if (!File.Exists(localFile)) File.Delete(localFile);
            File.Copy(filePath, localFile);

            logger.LogInformation($"Node {0} retrieved", NodeId);

            return new StorageRepsonse(true);

        }
        catch (Exception ex)
        {
            var msg = string.Format("Error getting NodeFile {0}: {1}", NodeId, ex.Message);
            logger.LogError(msg);
            return new StorageRepsonse(false, Message: msg);
        }
    }

    public async Task<StorageRepsonse> PutNodeFileAsync(int NodeId, CancellationToken ctk = default!)
    {

        try
        {
            using var ctx = await dbFactory.CreateDbContextAsync(ctk);
            var node = await ctx.Nodes
                .Include(n => n.RootNode)
                .FirstOrDefaultAsync(n => n.Id == NodeId, ctk);

            if (node is null) throw new Exception($"Node {NodeId} not found");

            if (node.RootNode is null || !node.RootNode.GroupId.HasValue) throw new Exception("Root node does not beldong to a group");

            if (string.IsNullOrEmpty(node.StorageId))
            {
                // create a new storygeId
                node.StorageId = SequentialGuidUtility.GetGuid().ToString();
            }

            // check local file in temp
            var localFile = Path.Combine(opts.Value.TempDataPath, NodeId.ToString());
            if (!File.Exists(localFile)) throw new Exception($"Local file not found: {localFile}");

            var fn = Path.Combine(GetNodePath(node.RootNode), node.StorageId);

            // delete storage file if exists
            if (File.Exists(fn)) File.Delete(fn);

            // copy tmp file to storgae
            File.Copy(localFile, fn);

            // save node
            node.File.LastStorageExportDate = DateTime.UtcNow;
            ctx.Nodes.Update(node);
            await ctx.SaveChangesAsync(ctk);

            return new StorageRepsonse(true, StorageId: node.StorageId);

        }
        catch (Exception ex)
        {
            var msg = string.Format("Error putting NodeFile {0}: {1}", NodeId, ex.Message);
            logger.LogError(msg);
            return new StorageRepsonse(false, Message: msg);
        }
    }



    public async Task<StorageRepsonse> PutNodeJsonAsync(int NodeId, CancellationToken ctk = default!)
    {
        try
        {
            using var ctx = await dbFactory.CreateDbContextAsync(ctk);
            var node = await ctx.Nodes
                .Include(n => n.ChildNodes)
                .Include(n => n.Annotations)
                .FirstOrDefaultAsync(n => n.Id == NodeId, ctk);

            return await PutNodeJsonAsync(node, ctk);
        }
        catch (Exception ex)
        {
            var msg = string.Format("Error putting NodeFile {0}: {1}", NodeId, ex.Message);
            logger.LogError(msg);
            return new StorageRepsonse(false, Message: msg);
        }
    }


    public async Task<StorageRepsonse> PutNodeJsonAsync(Node node, CancellationToken ctk = default!)
    {
        var jsonOpts = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        };

        var fn = Path.Combine(GetNodePath(node), $"{node.Id}.json");
        var str = JsonSerializer.Serialize(node, jsonOpts);
        await File.WriteAllTextAsync(fn, str, ctk);
        return new StorageRepsonse(true);
    }

     


    private string GetNodePath(Node node)
    {
        if( !Directory.Exists(StoragePath) ) throw new Exception("Root directory for local storage not found");

        var dir = Path.Combine(StoragePath, node.GroupId.ToString());
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        dir = Path.Combine(dir, node.Id.ToString());
        if( !Directory.Exists(dir)) Directory.CreateDirectory(dir); 

        return dir;
    }
}
