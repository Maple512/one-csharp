namespace OneS.Identityable.Users;

using System;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetList() => Ok();

    [HttpGet("{id}")]
    public IActionResult Get(Guid id) => Ok();

    [HttpDelete("id")]
    public IActionResult Delete(Guid id) => Ok();

    [HttpPut]
    public IActionResult Update() => Ok();
}
