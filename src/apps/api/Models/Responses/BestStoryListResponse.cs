using libs.Models;

namespace api.Models.Responses
{
    /// <summary>
    /// Response returned by API
    /// </summary>
    public class BestStoryListResponse
    {
        public NewsStory[] Stories { get; set; }    
    }
}
