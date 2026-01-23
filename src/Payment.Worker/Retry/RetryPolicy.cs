namespace Payment.Worker.Retry;

public static class RetryPolicy
{
    public static TimeSpan CalculateDelay(int retry)
    {
        const int baseSeconds = 2;
        const int maxSeconds = 60;

        var delay = Math.Pow(2, retry) * baseSeconds;
        return TimeSpan.FromSeconds(Math.Min(delay, maxSeconds));
    }
}