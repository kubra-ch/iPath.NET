using iPath.Data;
using iPath.Data.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using iPath.Application.Authentication;
using iPath.Application.Services.Storage;


namespace iPath.API.Controllers;

[ApiController]
[Route("api/files")]
public class FileController(IOptions<iPathConfig> opts, IDbContextFactory<NewDB> dbFactory, 
    IAuthManager am, IStorageService srvStorage, HttpClient httpClient)
    : ControllerBase
{
    [HttpGet("{id:int}")]
    [HttpGet("{id:int}/{filename}")]
    /* [Authorize] */
    public async Task<IActionResult> GetImage(int id)
    {
        am.Init(this.User);

        using var ctx = await dbFactory.CreateDbContextAsync();
        var node = await ctx.Nodes
            .Include(n => n.File)
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == id);

        if (node is null || node.File is null) return NotFound();


        // Check if file in local cache
        var filePath = Path.Combine(opts.Value.TempDataPath , $"{id}");
        if (!System.IO.File.Exists(filePath))
        {
            var resp = await srvStorage.GetNodeFileAsync(id);
            if( !resp.Success)
            {
                // fallback get from old system
                var backupfile = $"\\\\nas2\\BACKUP\\www\\ipath\\data";
                if(System.IO.Directory.Exists(backupfile))
                {
                    var grp = ((int)Math.Floor((decimal)node.Id / 1000)).ToString();
                    backupfile = Path.Combine(backupfile, grp);
                    backupfile = Path.Combine(backupfile, id.ToString() + ".data");
                    if( System.IO.File.Exists(backupfile))
                    {
                        System.IO.File.Copy(backupfile, filePath);
                    }
                }
            }
        }

        if (System.IO.File.Exists(filePath))
        {
            return File(System.IO.File.OpenRead(filePath), node.File.MimeType);
        }

        return NotFound();
    }
}

