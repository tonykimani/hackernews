using libs.Models;

namespace libs.Contracts
{
    /// <summary>
    /// Interface for a news service
    /// </summary>
    public interface INewsService
    {
        /// <summary>
        /// Test connectivity to HackerNews
        /// </summary>
        /// <returns></returns>
        Task<bool> Ping();

        /// <summary>
        /// Get a list of the best stories
        /// </summary>
        /// <param name="skipCount">If paging results this is the count already returned in previuous calls</param>
        /// <param name="maxCount">Maximum number of stories to return. If paging results this is the page size</param>
        /// <returns></returns>
        Task<NewsStory[]> ListBestStories(int? skipCount, int? maxCount = 100);

        /// <summary>
        /// Get a particular story (or null if not found)
        /// </summary>
        /// <param name="storyId">ID of the story</param>
        /// <returns></returns>
        Task<NewsStory?> GetStory(int storyId);
    }
}
