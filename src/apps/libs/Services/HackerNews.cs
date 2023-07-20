using libs.Constants;
using libs.contracts;
using libs.Contracts;
using libs.Models;
using libs.Utils;
using Microsoft.Extensions.Configuration;
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
        private static int _maxRequests;
        private readonly int _maxStoryAgeDays;
        private static SemaphoreSlim _throttle;
        private static readonly object _syncLock = new object();

        public HackerNewsService(HttpClient httpClient, ICache cache,IConfiguration configuration)
        {
            _httpClient = httpClient;
            _cache = cache;

            _maxRequests = configuration.GetValue("MAX_CONCURRENT_REQUESTS", 5);
            _maxStoryAgeDays = configuration.GetValue("MAX_STORY_AGE_DAYS", 7);
        }

        static SemaphoreSlim Throttle
        {
            get
            {
                if (_throttle == null)
                {
                    lock (_syncLock)
                    {
                        if (_throttle == null)
                        {
                            _throttle = new SemaphoreSlim(_maxRequests);
                        }
                    }
                }

                return _throttle;

            }
        }

        /// <summary>
        /// Call the base URI to test connectivity to hacker-news 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Ping()
        {
            try
            {
                using (new ThrottleUtils(Throttle))
                {
                    var result = await _httpClient.GetAsync("/");

                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
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
                    HttpResponseMessage response;

                    using (new ThrottleUtils(Throttle))
                    {
                        response = await _httpClient.GetAsync("v0/beststories.json");
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        storyIdsJson = await response.Content.ReadAsStringAsync();

                        var hoursToMidnight = (DateTimeOffset.Now.Midnight() - DateTimeOffset.Now);

                        await _cache.SetKey(KeyNames.STORIES, storyIdsJson, hoursToMidnight);
                        
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

                if (string.IsNullOrEmpty(storyJson))
                {
                    HttpResponseMessage response;

                    using (new ThrottleUtils(Throttle))
                    {
                        response = await _httpClient.GetAsync($"v0/item/{storyId}.json");
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        storyJson = await response.Content.ReadAsStringAsync();

                        await _cache.SetKey(storyCacheId, storyJson, TimeSpan.FromDays(_maxStoryAgeDays));
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
