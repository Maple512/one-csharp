namespace OneS.Identityable.Identitys;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class IdentityController : ControllerBase
{
    [HttpGet]
    public IActionResult GetList() => Ok();

    [HttpPost]
    [Route("register")]
    public IActionResult Register() => Ok();

    [HttpPost]
    [Route("login")]
    public IActionResult Login() => Ok();
}
