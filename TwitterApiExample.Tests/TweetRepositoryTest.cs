using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitterApiExample.Repositories;
using Xunit;

namespace TwitterApiExample.Tests;

public class TweetRepositoryTest
{
    public TweetRepositoryTest()
    {
    }

    private async Task UseRepo(string name, Action<TweetRepository> test)
    {
        await using (var repo = new TweetRepository($"Data Source={Regex.Replace(name, "[^a-zA-Z0-9]", "")};Mode=Memory;Cache=Shared"))
        {
            await repo.InitDb();
            test(repo);
        }
    }

    [Fact]
    public async Task Adding_Tweet_Should_Be_The_Same_Returned()
    {
        await UseRepo(nameof(Adding_Tweet_Should_Be_The_Same_Returned), async (repo) =>
        {
            var tweet = new Models.Tweet { Id = "Id1", AuthorId = "AuthorId2", Text = "Text3" };

            await repo.Save(tweet);
            var allTweets = await repo.GetAll();

            Assert.Single(allTweets);
            Assert.Equal(tweet.Id, allTweets[0].Id);
            Assert.Equal(tweet.AuthorId, allTweets[0].AuthorId);
            Assert.Equal(tweet.Text, allTweets[0].Text);
        });
    }

    [Fact]
    public async Task Adding_Many_Tweets_Should_Influence_Count()
    {
        await UseRepo(nameof(Adding_Many_Tweets_Should_Influence_Count), async (repo) =>
        {
            await repo.Save(new Models.Tweet { Id = "Id1", AuthorId = "AuthorId2", Text = "Text3" });
            await repo.Save(new Models.Tweet { Id = "Id2", AuthorId = "AuthorId22", Text = "Text33" });
            await repo.Save(new Models.Tweet { Id = "Id3", AuthorId = "AuthorId222", Text = "Text344" });

            var count = await repo.GetCount();

            Assert.Equal(3, count);
        });
    }

    [Fact]
    public async Task Should_Order_Hashtags_Instances_By_Count()
    {
        await UseRepo(nameof(Should_Order_Hashtags_Instances_By_Count), async (repo) =>
        {
            await repo.Save(new Models.Tweet { Id = "Id1", AuthorId = "AuthorId2", Text = "Hi #There I'm doing super #grEAt" });
            await repo.Save(new Models.Tweet { Id = "Id2", AuthorId = "AuthorId22", Text = "what is up #there folks! #dupe" });
            await repo.Save(new Models.Tweet { Id = "Id3", AuthorId = "AuthorId222", Text = "#dupe #dupe #duPe #Dupe" });

            var tags = await repo.GetTopHashtags(10);

            Assert.Equal(new List<string>() { "#dupe", "#there", "great" } , tags);
        });
    }
}