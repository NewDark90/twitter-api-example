namespace TwitterApiExample.Services;

public class TwitterStreamService<ITweetRespository> : IHostedService where ITweetRespository : new()
{
    ITweetRespository TweetRespository { get; set; } = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

