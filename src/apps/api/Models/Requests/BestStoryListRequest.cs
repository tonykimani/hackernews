namespace api.Models.Requests
{
    /// <summary>
    /// Request body by API
    /// </summary>
    public class BestStoryListRequest
    {
        public int MaxCount { get; set; }
        public int SkipCount{ get; set; }

    }
}
