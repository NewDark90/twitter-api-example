using TwitterApiExample.Repositories;
using Tweetinvi;
using Tweetinvi.Streaming.V2;

namespace TwitterApiExample.Services;

public class TwitterStreamService: IHostedService
{
    ITweetRepository TweetRepository { get; set; }
    ITwitterClient TwitterClient { get; set; }
    ISampleStreamV2 SampleStream { get; set; }
    ILogger Logger { get; set; }

    public TwitterStreamService(
        ITweetRepository tweetRepository,
        ITwitterClient twitterClient,
        ILogger logger
        )
    {
        TweetRepository = tweetRepository;
        TwitterClient = twitterClient;
        Logger = logger;
        SampleStream = TwitterClient.StreamsV2.CreateSampleStream();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await TweetRepository.InitDb();

        SampleStream.TweetReceived += (sender, args) =>
        {
            Logger.LogTrace("Tweet Received", args);

            TweetRepository.Save(new Models.Tweet()
            {
                Id = args.Tweet.Id,
                Text = args.Tweet.Text,
                AuthorId = args.Tweet.AuthorId
            });
        };

        await SampleStream.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        SampleStream.StopStream();
    }
}

