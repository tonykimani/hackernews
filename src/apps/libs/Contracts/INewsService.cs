namespace libs.Contracts
{
    public interface INewsService
    {
        /// <summary>
        /// Test connectivity
        /// </summary>
        /// <returns></returns>
        Task<bool> Ping();
    }
}
