namespace libs.Utils
{
    /// <summary>
    /// Semaphore wrapper than can be used in a using block; the dispose in this case disposes logical wait
    /// </summary>
    public class ThrottleUtils : IDisposable
    {
        private readonly SemaphoreSlim _semaphoreSlim;
        
        public ThrottleUtils(SemaphoreSlim semaphoreSlim)
        {
            _semaphoreSlim = semaphoreSlim;
            _semaphoreSlim.Wait();
        }

        public void Dispose()
        {
            _semaphoreSlim.Release();

        }


    }
}
