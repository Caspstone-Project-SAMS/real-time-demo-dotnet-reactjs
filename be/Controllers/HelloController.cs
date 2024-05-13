using Microsoft.AspNetCore.Mvc;

namespace WebSocketsSample.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController: ControllerBase
{
    [HttpGet]
    public IActionResult Hello()
    {
        return Ok("Hello");
    } 
}