using TwitterApiExample.Services;
using TwitterApiExample.Repositories;
using Tweetinvi;
using Tweetinvi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITweetRepository, TweetRepository>();
builder.Services.AddScoped<IReadOnlyConsumerCredentials, TwitterConsumerCredentials>();
builder.Services.AddScoped<ITwitterClient, TwitterClient>();
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
