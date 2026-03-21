using Microsoft.AspNetCore.Mvc;

namespace Speakeasy.Server.Controllers.ApiVersion1;

[Route("/ping")]
public class PingController : ControllerBase
{
    public IActionResult Get()
    {
        return Ok("Pong");
    }
}