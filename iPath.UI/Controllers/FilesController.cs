using iPath.Application.Configuration;
using iPath.Data.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace iPath.UI.Controllers;


[ApiController]
[Route("api/files")]
public class FilesController(IOptions<iPathConfig> opts, IPathDbContext ctx) : ControllerBase
{

    [HttpGet("{id:int}")]
    [HttpGet("{id:int}/{filename}")]
    public async Task<IActionResult> GetImage(int id)
    {
        var filePath = Path.Combine(opts.Value.DataPath, $"{id}");
        var node = await ctx.Nodes
            .Include(n => n.File)
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == id);

        if (node is null || node.File is null || !System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        return File(System.IO.File.OpenRead(filePath), node.File.MimeType);
    }

}
