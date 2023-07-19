using libs.Contracts;
using libs.Models;
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
        private readonly JsonSerializerSettings _serializerSettings;

        public HackerNewsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _serializerSettings = new JsonSerializerSettings();
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

        public async Task<NewsStory[]> ListBestStories(int? skipCount, int? maxCount = 100)
        {
            var stories = new List<NewsStory>();

            try
            {
                var response = await _httpClient.GetAsync("v0/beststories.json");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var results = JsonConvert.DeserializeObject<IEnumerable<int>>(content, _serializerSettings);

                    if (skipCount != null)
                    {
                        results = results.Skip(skipCount.Value);
                    }

                    if (maxCount != null)
                    {
                        results = results.Take(maxCount.Value);
                    }

                    foreach (var storyId in results)
                    {
                        var story = await GetStory(storyId);

                        if (story != null)
                        {
                            stories.Add(story);
                        }

                    }

                }
                else
                {
                    Log.Warning($"Unable to get best stories from url {response.RequestMessage.RequestUri} because {response.StatusCode} {response.ReasonPhrase}");
                }

            }
            catch (Exception ex)
            {
                Log.Debug($"Failed while downloading best stories .{ex.Message}.{ex.StackTrace}", ex);
            }


            return stories.OrderByDescending(story => story.Score).ToArray();
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
                var response = await _httpClient.GetAsync($"v0/item/{storyId}.json");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    return  JsonConvert.DeserializeObject<NewsStory>(content, _serializerSettings);
                }
                else
                {
                    Log.Warning($"Unable to get story {storyId} from url {response.RequestMessage.RequestUri} because {response.StatusCode} {response.ReasonPhrase}");
                }

            }
            catch (Exception ex)
            {
                Log.Debug($"Failed while downloading story id {storyId} .{ex.Message}.{ex.StackTrace}", ex);
            }

            return null;
        }
    }
}
