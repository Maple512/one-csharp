namespace OneS.Identityable.Identitys;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class IdentityController : ControllerBase
{
    [HttpGet]
    public IActionResult GetList()
    {
        return Ok();
    }

    [HttpPost]
    [Route("register")]
    public IActionResult Register()
    {
        return Ok();
    }

    [HttpPost]
    [Route("login")]
    public IActionResult Login()
    {
        return Ok();
    }
}
