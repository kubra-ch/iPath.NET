using iPath.Application.Configuration;
using iPath.Application.Features;
using iPath.Application.Services;
using iPath.Data.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace iPath.UI.Controllers;


[ApiController]
[Route("api/files")]
public class FilesController(IOptions<iPathConfig> opts, IDbContextFactory<IPathDbContext> dbFactory) : ControllerBase
{

    [HttpGet("{id:int}")]
    [HttpGet("{id:int}/{filename}")]
    public async Task<IActionResult> GetImage(int id)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        var node = await ctx.Nodes
            .Include(n => n.File)
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == id);

        if (node is null || node.File is null) return NotFound();

        // Filepath from live system
        var filePath = Path.Combine(opts.Value.DataPath, $"{id}");
        if( System.IO.File.Exists(filePath) )
        {
            return File(System.IO.File.OpenRead(filePath), node.File.MimeType);
        }

        // fallback to backup storage
        if( !string.IsNullOrEmpty(opts.Value.DataPathReadonly) && System.IO.Directory.Exists(opts.Value.DataPathReadonly))
        {
            var folder = ((int)Math.Floor((decimal)id / 1000)).ToString();
            filePath = Path.Combine(opts.Value.DataPathReadonly, $"{folder}\\{id}");
            if (System.IO.File.Exists(filePath))
            {
                return File(System.IO.File.OpenRead(filePath), node.File.MimeType);
            }
        }

        return NotFound();
    }



    [HttpGet("thumb/{id:int}")]
    public async Task GetThumb(int id, [FromServices] IThumbImageService srvThumb)
    {
        using var ctx = await dbFactory.CreateDbContextAsync();
        var node = await ctx.Nodes
            .Include(n => n.File)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (node != null && node.File != null)
        {
            // Filepath from live system
            var filePath = Path.Combine(opts.Value.DataPath, $"{id}");
            if (!System.IO.File.Exists(filePath))
            {
                // fallback to backup of iPath 2 data
                var folder = ((int)Math.Floor((decimal)id / 1000)).ToString();
                filePath = Path.Combine(opts.Value.DataPathReadonly, $"{folder}\\{id}.data");
            }

            if (System.IO.File.Exists(filePath))
            {
                srvThumb.UpdateNode(node.File, filePath);
                ctx.Update(node);
                await ctx.SaveChangesAsync();

                byte[] bytes = Convert.FromBase64String(node.File.ThumbData);
                using var os = this.Response.Body;
                this.Response.ContentType = "image/jpeg";
                this.Response.ContentLength = bytes.Length;
                await os.WriteAsync(bytes);
                await os.FlushAsync();
                os.Close();
            }
            else
            {
                Console.Write("file not found: " + filePath);
            }
        }
    }
}
