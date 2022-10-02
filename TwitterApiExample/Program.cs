using TwitterApiExample.Services;
using TwitterApiExample.Repositories;
using Tweetinvi;
using Tweetinvi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITweetRepository, TweetRepository>();
builder.Services.AddSingleton<IReadOnlyConsumerCredentials, TwitterConsumerCredentials>();
builder.Services.AddSingleton<ITwitterClient, TwitterClient>();
builder.Services.AddHostedService<TwitterStreamService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
