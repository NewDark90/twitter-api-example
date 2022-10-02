using Tweetinvi.Models;

namespace TwitterApiExample.Services
{
    public class TwitterConsumerCredentials : IReadOnlyConsumerCredentials
    {
        public TwitterConsumerCredentials(IConfiguration config)
        {
            BearerToken = config["Twitter:BearerToken"];
            ConsumerKey = config["Twitter:Key"];
            ConsumerSecret = config["Twitter:Secret"];
        }


        public string BearerToken { get; private set; }

        public string ConsumerKey { get; private set; }

        public string ConsumerSecret { get; private set; }
    }
}
