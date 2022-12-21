namespace OneS.Identityable.Users;

using System;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetList()
    {
        return Ok();
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        return Ok();
    }

    [HttpDelete("id")]
    public IActionResult Delete(Guid id)
    {
        return Ok();
    }

    [HttpPut]
    public IActionResult Update()
    {
        return Ok();
    }
}
