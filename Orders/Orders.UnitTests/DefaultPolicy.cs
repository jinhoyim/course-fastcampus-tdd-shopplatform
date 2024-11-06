using Polly;

namespace Orders;

public static class DefaultPolicy
{
    private static readonly Random s_random = new();
    public static IAsyncPolicy Instance { get; } = Policy.Handle<System.Exception>().WaitAndRetryAsync(5, CalculateDelay);

    private static TimeSpan CalculateDelay(int retries)
    {
        int delayMilliseconds = 100;
        for (int i = 0; i < retries; i++)
        {
            delayMilliseconds *= 2;
            delayMilliseconds += s_random.Next(20);
        }
        return TimeSpan.FromMilliseconds(delayMilliseconds);
    }
}