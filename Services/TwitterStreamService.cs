using TwitterSharp;
using TwitterSharp.Client;

namespace TwitterApiExample.Services;

public class TwitterStreamService<ITweetRespository> : IHostedService where ITweetRespository : new()
{
    //Would probably have this DI'd but not setting up a whole framework for it.
    //The AddHostedService just takes a generic class, not an instance, so it isn't super easy to just instantiate one in there.
    ITweetRespository TweetRespository { get; set; } = new();
    TwitterClient TwitterClient { get; set; } = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

