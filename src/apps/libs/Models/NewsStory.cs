using libs.Converters;
using Newtonsoft.Json;
using System.ComponentModel;

namespace libs.Models
{
    /// <summary>
    /// A news story
    /// </summary>
    public class NewsStory
    {
        [JsonProperty("id")]
        public string StoryId { get; set; }

        [JsonProperty("by")]
        public string PostedBy { get; set; }

        [JsonProperty("descendants")]
        public int DescendantCount { get; set; }

        public int CommentCount { get { return DescendantCount; } }

        [JsonProperty("kids")]
        public int[] ChildIds { get; set; }
        
        public int Score { get; set; }

        [JsonProperty("time")]
        [JsonConverter(typeof(TimeConverter))]
        public int PublishedOn { get; set; }

        public string Title { get; set; }

        [JsonProperty("type")]
        public string ItemType { get; set; }

        [JsonProperty("uri")]
        public string Url { get; set; }
    }
}
