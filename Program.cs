using TwitterApiExample.Services;
using TwitterApiExample.Repositories;
using Tweetinvi;
using Tweetinvi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITweetRepository, TweetRepository>();
builder.Services.AddScoped<IReadOnlyConsumerCredentials, TwitterConsumerCredentials>();
builder.Services.AddScoped<ITwitterClient, TwitterClient>();
builder.Services.AddHostedService<TwitterStreamService>();

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
