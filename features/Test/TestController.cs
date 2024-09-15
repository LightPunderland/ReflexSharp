using Microsoft.AspNetCore.Mvc;

namespace Features.Test
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "works gud" });
        }
    }
}
