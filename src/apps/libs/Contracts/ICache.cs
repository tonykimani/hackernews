namespace libs.contracts
{
    public interface ICache
    {
        /// <summary>
        /// Test connectivity
        /// </summary>
        /// <returns></returns>
        Task<bool> Ping();
    }
}
