using iPath.Application.Authentication;
using iPath.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace iPath.Application.Controllers;

[ApiController]
[Route("api/test")]
public class TestController(IAuthManager am) : ControllerBase
{
    protected async Task<UserProfile> getProfileAsyc()
    {
        am.Init(User);
        return await am.GetProfileAync();
    }


    [HttpGet("auth")]
    [Authorize]
    public async Task<ActionResult> AuthTest()
    {
        var usr = await getProfileAsyc();
        return Ok($"yes, {usr.FirstName} {usr.FamilyName}, you can");
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public ActionResult AdminTest()
    {
        return Ok("yes, you are an admin");
    }

    [HttpGet("noauth")]
    public ActionResult NoAuthTest()
    {
        return Ok("everyone can");
    }

}
