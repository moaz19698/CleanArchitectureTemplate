namespace CleanArchitecture.Common.Operation
{
    public static class SemaphoreSlimExtension
    {
        public static async Task<T> WaitThenReleaseAsync<T>(this SemaphoreSlim semaphore, Func<Task<T>> function, CancellationToken cancellationToken)
        {
            T result;
            try
            {
                await semaphore.WaitAsync(cancellationToken);
                result = await function();
            }
            finally
            {
                semaphore.Release();
            }

            return result;
        }
        public static async Task WaitThenReleaseAsync(this SemaphoreSlim semaphore, Func<Task> function, CancellationToken cancellationToken)
        {
            try
            {
                await semaphore.WaitAsync(cancellationToken);
                await function();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
