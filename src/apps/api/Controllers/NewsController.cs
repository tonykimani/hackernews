using api.Models.Requests;
using api.Models.Responses;
using libs.Contracts;
using Microsoft.AspNetCore.Mvc;
using Serilog;
namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly int _maxMaxCount;

        public NewsController(INewsService newsService, IConfiguration configuration)
        {
            _newsService = newsService;
            _maxMaxCount = configuration.GetValue("MAX_STORY_COUNT", 100);
        }

        [HttpPost("BestStories")]
        public async Task<IActionResult> ListBestStories(BestStoryListRequest request)
        {
            try
            {
                if (request.MaxCount > _maxMaxCount)
                {
                    return BadRequest(new ErrorResponse { Code="100", Message = "MaxCount threshold exceeded." });
                }

                var stories = await _newsService.ListBestStories(request.SkipCount, request.MaxCount);

                return Ok(stories.Select(x => new StoryResponse(x)).ToArray());
            }
            catch (Exception ex)
            {
                Log.Error($"Failed while listing best stories.{ex.Message}.{ex.StackTrace}", ex);

                return BadRequest(new ErrorResponse { Code="101", Message="Server Error"});
            }
        }
    }
}