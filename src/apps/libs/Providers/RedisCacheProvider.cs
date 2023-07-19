using libs.contracts;

namespace libs.Providers
{
    public class RedisCacheProvider : ICache
    {
        public Task<bool> Ping()
        {
            throw new NotImplementedException();
        }
    }
}
