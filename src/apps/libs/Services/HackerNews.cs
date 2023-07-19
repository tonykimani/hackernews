using libs.Contracts;

namespace api.Services
{
    public class HackerNewsService : INewsService
    {
        public Task<bool> Ping()
        {
            return Task.FromResult( true);
        }
    }
}
