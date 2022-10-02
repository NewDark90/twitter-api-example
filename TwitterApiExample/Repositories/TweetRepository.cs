using Microsoft.Data.Sqlite;
using System.Linq;
using System.Text.RegularExpressions;
using TwitterApiExample.Models;

namespace TwitterApiExample.Repositories;

public class TweetRepository : ITweetRepository
{
    private const string TweetTable = "tweets";
    private const string TweetTableId = "id";
    private const string TweetTableContents = "contents";
    private const string TweetTableAuthorId = "authorId";

    private const string HashtagCountTable = "hashtags";
    private const string HashtagCountTableHashtag = "hashtag";
    private const string HashtagCountTableCount = "count";

    //Slightly modified from here: https://stackoverflow.com/a/38383605/2283050
    //Is it perfect? Based on what I see on the internet, it takes a crazy amount of code to parse hashtags out of a tweet.
    //The minor fix is ensuring the tag either begins the tweet, or is proceeded by whitespace
    private readonly Regex HashtagRegex = new(@"/(^|\s)#(\w*[0-9a-zA-Z]+\w*[0-9a-zA-Z])/g");

    //Only setting this data store to be static as it would mimic a more permanent storage
    //static List<Tweet> Tweets { get; set; } = new List<Tweet>();

    public TweetRepository()
    {

    }


    public async Task InitDb()
    {
        using (var connection = new SqliteConnection("Data Source=:memory:"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            //Is the hashtag count table the best way of handling this? Absolutely not.
            //Ideally hashtags would probably be in their own table with a many to many table linking them, but that's beyond the scope of a small demo like this.
            command.CommandText =
            @$"
                CREATE TABLE {TweetTable}  (
                    {TweetTableId} TEXT NOT NULL PRIMARY KEY,
                    {TweetTableContents} TEXT NOT NULL,
                    {TweetTableAuthorId} TEXT NOT NULL
                );

                CREATE TABLE {HashtagCountTable}  (
                    {HashtagCountTableHashtag} TEXT NOT NULL PRIMARY KEY,
                    {HashtagCountTableCount} INTEGER NOT NULL
                );
            ";
            await command.ExecuteNonQueryAsync();
        }
    }

    private async Task IncrementHashtagDb(Tweet tweet)
    {
        var hashtags = HashtagRegex.Matches(tweet.Text ?? "").Select(m => m.Value).ToList();

        using (var connection = new SqliteConnection("Data Source=:memory:"))
        {
            connection.Open();
            foreach(var hashtag in hashtags)
            {
                var command = connection.CreateCommand();
                command.CommandText =
                @$"
                    INSERT INTO {HashtagCountTable} VALUES ($hashtag, 1);
                    ON CONFLICT({HashtagCountTableHashtag}) DO 
                        UPDATE SET {HashtagCountTableCount} = {HashtagCountTableCount} + 1;
                ";
                command.Parameters.AddWithValue("$hashtag", hashtag.ToLower());
                await command.ExecuteNonQueryAsync();
            }
        }
    }

    private async Task SaveTweetDb(Tweet tweet)
    {
        using (var connection = new SqliteConnection("Data Source=:memory:"))
        {
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @$"
                INSERT INTO {TweetTable} ({TweetTableId}, {TweetTableContents}, {TweetTableAuthorId})
                VALUES ($id, $contents, $authorid)
            ";
            command.Parameters.AddWithValue("$id", tweet.Id);
            command.Parameters.AddWithValue("$contents", tweet.Text);
            command.Parameters.AddWithValue("$authorid", tweet.AuthorId);
            await command.ExecuteNonQueryAsync();
        }
    }

    private async Task<IList<Tweet>> GetAllTweetsDb()
    {
        var tweets = new List<Tweet>();

        using (var connection = new SqliteConnection("Data Source=:memory:"))
        {
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @$"
                SELECT ({TweetTableId}, {TweetTableContents}, {TweetTableAuthorId}) FROM {TweetTable}
            ";
            var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                var tweet = new Tweet()
                {
                    Id = reader[TweetTableId]?.ToString() ?? "",
                    Text = reader[TweetTableContents]?.ToString() ?? "",
                    AuthorId = reader[TweetTableAuthorId]?.ToString() ?? "",
                };
                if (!String.IsNullOrEmpty(tweet.Id))
                { 
                    tweets.Add(tweet); 
                }
            }
        }

        return tweets;
    }

    private async Task<int> GetTweetCountDb()
    {
        using (var connection = new SqliteConnection("Data Source=:memory:"))
        {
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @$"
                SELECT COUNT(*) FROM {TweetTable}
            ";
            return (int?)await command.ExecuteScalarAsync() ?? 0;
        }
    }


    public async Task Save(Tweet tweet)
    {
        await IncrementHashtagDb(tweet);
        await SaveTweetDb(tweet);
    }

    public async Task<IList<Tweet>> GetAll()
    {
        return await GetAllTweetsDb();
    }

    public async Task<int> GetCount()
    {
        return await GetTweetCountDb();
    }

    public async Task<IList<string>> GetTopHashtags(int top = 10)
    {
        var tags = new List<string>();

        using (var connection = new SqliteConnection("Data Source=:memory:"))
        {
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @$"
                SELECT ({HashtagCountTableHashtag}) 
                FROM {HashtagCountTable}
                ORDER BY {HashtagCountTableCount} DESC
                LIMIT {top}
            ";
            var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                var hashtag = reader[HashtagCountTableHashtag]?.ToString() ?? "";
                if (!String.IsNullOrEmpty(hashtag))
                {
                    tags.Add(hashtag);
                }
            }
        }

        return tags;
    }
}

