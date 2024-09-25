namespace CleanArchitecture.Common.Operation
{
    public static class OperationExecuter
    {
        public static async Task<Exception?> RetryAsync(Func<Task> func, int numberOfRetries = 3, int waitTime = 500)
        {
            var tries = 0;
            Exception? exception = null;

            while (tries <= numberOfRetries)
            {
                try
                {
                    await func();
                    return (null);
                }
                catch (Exception ex)
                {
                    exception = ex;
                    tries++;
                    await Task.Delay(waitTime);
                }
            }

            return (exception);
        }
        public static async Task<TResult?> RetryAsync<TResult>(Func<Task<TResult>> func, int numberOfRetries = 3, int waitTime = 500)
        {
            var tries = 0;

            while (tries <= numberOfRetries)
            {
                try
                {
                    return await func();
                }
                catch
                {
                    tries++;
                    await Task.Delay(waitTime);
                }
            }

            return default;
        }
    }
}
