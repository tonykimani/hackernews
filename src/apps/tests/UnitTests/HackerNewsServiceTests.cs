using api.Services;
using libs.Constants;
using libs.contracts;
using libs.Contracts;
using libs.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net;

namespace tests.UnitTests
{
    [TestFixture]
    public class HackerNewsServiceTests
    {
        private Mock<ICache> _mockCache;
        private Mock<HttpMessageHandler> _mockMessageHandler;
        private INewsService _hackerNews;
        

        [SetUp]
        public void SetUp()
        {
            
            _mockCache = new Mock<ICache>();            
            _mockMessageHandler = new Mock<HttpMessageHandler>();

            var httpClient = new HttpClient(_mockMessageHandler.Object);
            httpClient.BaseAddress = new Uri("http://unittests.local");

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();

            _hackerNews = new HackerNewsService(httpClient, _mockCache.Object, configuration); 
        }

        

        private List<int> TestStoryIds()
        {

            var storyIdList = new List<int>();
            storyIdList.Add(0);
            storyIdList.Add(1);
            storyIdList.Add(2);

            return storyIdList;
        }

        private void MockHttpContent(string content)
        {
            _mockMessageHandler.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(content)
               });
            
        }
        [Test]
        public void PingTest()
        {

            MockHttpContent("<html/>");

            var result = _hackerNews.Ping().Result;
            Assert.IsTrue(result, "Should return true");

            _mockMessageHandler.Verify();
        }

        [Test]
        public void ListStoryIdsFromCache()
        {
            var storyids = TestStoryIds();

            _mockCache.Setup(c => c.GetKey(KeyNames.STORIES)).Returns(JsonConvert.SerializeObject(storyids));
            
            var result = _hackerNews.ListBestStoryIds().Result;

            Assert.IsNotNull(result, "Array should not be null");
            Assert.AreEqual(result.Length, storyids.Count, "Array should have items");

            _mockCache.Verify(m => m.GetKey(KeyNames.STORIES), Times.Once());
        }

        [Test]
        public void ListStoryIdsFromHttp()
        {
            var storyids = TestStoryIds();
            var storiesJson = JsonConvert.SerializeObject(storyids);
            MockHttpContent(storiesJson);

            _mockCache.Setup(c => c.GetKey(KeyNames.STORIES)).Returns(string.Empty);
            _mockCache.Setup(c => c.SetKey(KeyNames.STORIES, storiesJson, It.IsAny<TimeSpan>()));

            var result = _hackerNews.ListBestStoryIds().Result;

            Assert.IsNotNull(result, "Array should not be null");
            Assert.AreEqual(result.Length, storyids.Count, "Array should have items");

            _mockCache.Verify(m=>m.GetKey(KeyNames.STORIES),Times.Once());
            _mockCache.Verify(m => m.SetKey(KeyNames.STORIES, It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once());
            _mockMessageHandler.Verify();
        }

        [Test]
        public void GetStoryFromCache()
        {
            var story = new NewsStory { Id = 1 };

            _mockCache.Setup(c => c.GetKey(KeyNames.STORY_PREFIX+ story.Id)).Returns(JsonConvert.SerializeObject(story));

            var result = _hackerNews.GetStory(1).Result;

            Assert.IsNotNull(result, "story should not be null");
            Assert.AreEqual(result.Id, 1, "correct story id");

            _mockCache.Verify(m => m.GetKey(KeyNames.STORY_PREFIX + story.Id), Times.Once());
        }

        [Test]
        public void GetStoryFromHttp()
        {
            var story = new NewsStory { Id = 1 };
            var storyJson = JsonConvert.SerializeObject(story);
            MockHttpContent(storyJson);

            _mockCache.Setup(c => c.GetKey(KeyNames.STORY_PREFIX + story.Id)).Returns(JsonConvert.SerializeObject(story));
            _mockCache.Setup(c => c.SetKey(KeyNames.STORY_PREFIX + story.Id, storyJson, It.IsAny<TimeSpan>()));

            var result = _hackerNews.GetStory(1).Result;


            Assert.IsNotNull(result, "story should not be null");
            Assert.AreEqual(result.Id, 1, "correct story id");

            _mockCache.Verify(m => m.GetKey(KeyNames.STORY_PREFIX + story.Id), Times.Once());
            _mockMessageHandler.Verify();
        }



    }
}
