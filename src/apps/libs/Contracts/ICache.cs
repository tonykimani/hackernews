namespace libs.contracts
{
    public interface ICache
    {
        /// <summary>
        /// Test connectivity
        /// </summary>
        /// <returns></returns>
        Task<bool> Ping();

        /// <summary>
        /// Return a value from the cache or null if not in cache
        /// </summary>
        /// <param name="key">The cached value or null if doesn't exist</param>
        /// <returns></returns>
        string? GetKey(string key);

        /// <summary>
        /// Add a value to the cache and set its expiry window
        /// </summary>
        /// <param name="key">A retrieval key</param>
        /// <param name="value">The cached value</param>
        /// <param name="expiry">Time after which the cache will remove the key</param>
        Task SetKey(string key, string value, TimeSpan expiry);
    }
}
