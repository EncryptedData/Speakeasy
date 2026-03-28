using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Speakeasy.Server.Controllers.ApiVersion1;

[Authorize]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("/api/v1/[controller]")]
public abstract class BaseV1ApiController : ControllerBase
{
}