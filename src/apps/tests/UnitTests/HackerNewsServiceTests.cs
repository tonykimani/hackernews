using api.Services;
using libs.Contracts;
using NUnit.Framework;

namespace tests.UnitTests
{
    [TestFixture]
    public class HackerNewsServiceTests
    {
        private INewsService _hackerNews;

        [SetUp]
        public void SetUp()
        {
            _hackerNews = new HackerNewsService();
        }

        [Test]
        public void PingTest()
        {
            var result = _hackerNews.Ping().Result;

            Assert.IsFalse(result, "Should return true");
        }

    }
}
