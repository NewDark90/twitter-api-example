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

    private const int maxMisses = 4;
    private int successiveMisses = 0;

    public TwitterStreamService(
        ITweetRepository tweetRepository,
        ITwitterClient twitterClient,
        ILogger<TwitterStreamService> logger
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
            if (args?.Tweet is null)
            { 
                Logger.LogWarning("Tweet doesn't exist on receive.", args.Json);
                successiveMisses++;
                if (successiveMisses >= maxMisses)
                {
                    throw new Exception($"{successiveMisses} tweets 'received' in a row but no tweet returned. Response: {args.Json}");
                }
                return; 
            }

            successiveMisses = 0;
            Logger.LogTrace("Tweet Received", args.Json);

            TweetRepository.Save(new Models.Tweet()
            {
                Id = args.Tweet.Id,
                Text = args.Tweet.Text,
                AuthorId = args.Tweet.AuthorId,
                Hashtags = args.Tweet.Entities?.Hashtags?.Select(h => h.Tag).ToList() ?? new List<string>()
            });
        };

        _ = SampleStream.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        SampleStream.StopStream();
        await TweetRepository.DisposeAsync();
    }
}

