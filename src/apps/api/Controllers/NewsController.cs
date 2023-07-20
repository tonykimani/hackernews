using api.Models.Requests;
using api.Models.Responses;
using libs.contracts;
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
        public async Task<ActionResult<BestStoryResponse[]>> ListBestStories(BestStoryListRequest request)
        {
            try
            {
                Log.Information("ListBestStories running");

                if (request.MaxCount > _maxMaxCount)
                {
                    return BadRequest(new ErrorResponse { Code="100", Message = "MaxCount threshold exceeded." });
                }

                if (request.MaxCount == 0)
                {
                    request.MaxCount = 10;
                }

                var stories = new List<BestStoryResponse>();
                var storyids = await _newsService.ListBestStoryIds();

                foreach(var storyid in storyids.Skip(request.SkipCount).Take(request.MaxCount))
                {
                    var story = await _newsService.GetStory(storyid);
                    if (story != null)
                    {
                        stories.Add(new BestStoryResponse(story));
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