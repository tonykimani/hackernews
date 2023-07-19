using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {

        [HttpGet("Hello")]
        public async Task<IActionResult> Hello()
        {
            try
            {
                //check that hacker news is reachable

                return Ok();

            }
            catch (Exception ex)
            {
                Log.Error($"Failed while greeting.{ex.Message}.{ex.StackTrace}", ex);

                return BadRequest();
            }
        }
    }
}
