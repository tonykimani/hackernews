using Newtonsoft.Json;

namespace libs.Models
{
    /// <summary>
    /// A news story
    /// </summary>
    public class NewsStory
    {
        
        public int Id { get; set; }

        [JsonProperty("by")]        
        public string PostedBy { get; set; }

        [JsonProperty("descendants")]        
        public int CommentCount { get; set; }

        
        [JsonProperty("kids")]
        public int[] ChildIds { get; set; }
        
        public int Score { get; set; }

        [JsonProperty("time")]
        public int PublishedOn { get; set; }

        public string Title { get; set; }

        [JsonProperty("type")]
        public string ItemType { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
        
    }
}
