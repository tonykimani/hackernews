using Microsoft.AspNetCore.Mvc;
using Serilog;
namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsController : ControllerBase
    {
 

        public NewsController()
        {
           
        }

        [HttpPost("BestStories")]
        public async Task<IActionResult> ListBestStories()
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed while listing best stories.{ex.Message}.{ex.StackTrace}", ex);

                return BadRequest();
            }
        }
    }
}