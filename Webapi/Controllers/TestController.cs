using Microsoft.AspNetCore.Mvc;
using Webapi.Attributes;

namespace Webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : BaseApiController
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok( new { Message = "" });
    }
}