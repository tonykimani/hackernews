using libs.Constants;
using libs.contracts;
using libs.Contracts;
using libs.Models;
using libs.Utils;
using Newtonsoft.Json;
using Serilog;

namespace api.Services
{
    /// <summary>
    /// Service that wraps the hacker-news API calls for internal use
    /// </summary>
    public class HackerNewsService : INewsService
    {
        private readonly HttpClient _httpClient;
        private readonly ICache _cache;
        
        public HackerNewsService(HttpClient httpClient, ICache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        /// <summary>
        /// Call the base URI to test connectivity to hacker-news 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Ping()
        {
            try
            {
                var result = await _httpClient.GetAsync("/");

                if (result.IsSuccessStatusCode)
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                Log.Debug($"Failed while pinging hacker news .{ex.Message}.{ex.StackTrace}", ex);
            }

            return false;
        }

        public async Task<int[]> ListBestStoryIds()
        {
            try
            {
                var storyIdsJson = _cache.GetKey(KeyNames.STORIES);

                if (string.IsNullOrEmpty(storyIdsJson))
                {
                    var response = await _httpClient.GetAsync("v0/beststories.json");

                    if (response.IsSuccessStatusCode)
                    {
                        storyIdsJson = await response.Content.ReadAsStringAsync();

                        var hoursToUTCMidnight = (DateTimeOffset.UtcNow.Midnight() - DateTimeOffset.UtcNow);

                        await _cache.SetKey(KeyNames.STORIES, storyIdsJson, hoursToUTCMidnight);
                        
                    }
                    else
                    {
                        Log.Warning($"Unable to get best stories from url {response.RequestMessage.RequestUri} because {response.StatusCode} {response.ReasonPhrase}");

                        storyIdsJson = "[]";
                    }
                }

                return JsonConvert.DeserializeObject<int[]>(storyIdsJson).Or<int[]>(Array.Empty<int>());

            }
            catch (Exception ex)
            {
                Log.Debug($"Failed while downloading best stories .{ex.Message}.{ex.StackTrace}", ex);
            }


            return new int[] { }; 
        }

        /// <summary>
        /// Returns a newstory or null (if can't be found)
        /// </summary>
        /// <param name="storyId"></param>
        /// <returns></returns>
        public async Task<NewsStory?> GetStory(int storyId)
        {
            try
            {
                var storyCacheId = $"{KeyNames.STORY_PREFIX}{storyId}";

                var storyJson = _cache.GetKey(storyCacheId);

                if (storyJson == null)
                {
                    var response = await _httpClient.GetAsync($"v0/item/{storyId}.json");

                    if (response.IsSuccessStatusCode)
                    {
                        storyJson = await response.Content.ReadAsStringAsync();

                        await _cache.SetKey(KeyNames.STORIES, storyJson, TimeSpan.FromDays(7));
                    }
                    else
                    {
                        Log.Warning($"Unable to get story {storyId} from url {response.RequestMessage.RequestUri} because {response.StatusCode} {response.ReasonPhrase}");

                        storyJson = "{}";
                    }
                }

                return JsonConvert.DeserializeObject<NewsStory>(storyJson);

            }
            catch (Exception ex)
            {
                Log.Debug($"Failed while downloading story id {storyId} .{ex.Message}.{ex.StackTrace}", ex);
            }

            return null;
        }
    }
}
