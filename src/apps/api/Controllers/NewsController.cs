using api.Models.Requests;
using api.Models.Responses;
using libs.Constants;
using libs.contracts;
using libs.Contracts;
using libs.Models;
using libs.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly ICache _cache;
        private readonly int _maxMaxCount;

        public NewsController(INewsService newsService, ICache cache, IConfiguration configuration)
        {
            _newsService = newsService;
            _cache = cache;
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

                var stories = new List<StoryResponse>();
                var storyids = await _newsService.ListBestStoryIds();

                foreach(var storyid in storyids.Skip(request.SkipCount).Take(request.MaxCount))
                {
                    var story = await _newsService.GetStory(storyid);
                    if (story != null)
                    {
                        stories.Add(new StoryResponse(story));
                    }
                }

                return Ok(stories.OrderByDescending(story => story.Score));
                 
            }
            catch (Exception ex)
            {
                Log.Error($"Failed while listing best stories.{ex.Message}.{ex.StackTrace}", ex);

                return BadRequest(new ErrorResponse { Code="101", Message="Server Error"});
            }
        }
    }
}