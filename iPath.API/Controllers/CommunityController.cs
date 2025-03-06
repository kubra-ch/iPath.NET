using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPath.API.Controllers;


[ApiController]
[Route("api/communities")]
public class CommunityController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCommunities()
    {
        return Ok("xxx");
    }

}
