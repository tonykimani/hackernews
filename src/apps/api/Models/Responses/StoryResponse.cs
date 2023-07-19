using libs.Models;

namespace api.Models.Responses
{
    /// <summary>
    /// Client facing story (renames some of the properties from HackerNews)
    /// We could avoid having to have this by using GraphQL but you asked for REST
    /// </summary>
    public class StoryResponse
    {
        private readonly NewsStory _story;

        public StoryResponse(NewsStory story)
        {
            _story = story;
        }

        public string Title { get { return _story.Title; } }
        public string Uri { get { return _story.Url; } }

        public string PostedBy { get {  return _story.PostedBy; } } 

        public DateTimeOffset Time { get { return DateTimeOffset.FromUnixTimeSeconds(_story.PublishedOn); } }

        public int Score { get {  return _story.Score; } }

        public int CommentCount { get { return _story.CommentCount; } }

    }
}
