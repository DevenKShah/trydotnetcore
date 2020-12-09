using Microsoft.AspNetCore.Mvc;

namespace MyWebApi.Controllers
{
    [ApiVersion("1")]
    [ApiVersion("2")]
    [ApiController]
    [Route("api/[Controller]")]
    public class PersonController : ControllerBase
    {
        [MapToApiVersion("1")]
        [ApiExplorerSettings(GroupName = "v1")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new PersonResponse("Deven", "v1"));
        }

        [MapToApiVersion("2")]
        [ApiExplorerSettings(GroupName = "v2")]
        [HttpGet]
        public IActionResult GetV2()
        {
            return Ok(new PersonResponse("Suman", "v2"));
        }
    }

    public record PersonResponse(string Name, string Version);
}