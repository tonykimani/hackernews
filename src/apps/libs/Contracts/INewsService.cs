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
        /// Get a list of the best story ids
        /// </summary>
        /// <returns></returns>
        Task<int[]> ListBestStoryIds();

        /// <summary>
        /// Get a particular story (or null if not found)
        /// </summary>
        /// <param name="storyId">ID of the story</param>
        /// <returns></returns>
        Task<NewsStory?> GetStory(int storyId);
    }
}
