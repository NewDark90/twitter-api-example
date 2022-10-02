using Microsoft.AspNetCore.Mvc;
using TwitterApiExample.Models;
using TwitterApiExample.Repositories;

namespace TwitterApiExample.Controllers;

[ApiController]
[Route("[controller]")]
public class TweetsController : ControllerBase
{
    private ILogger<TweetsController> Logger { get; set; }
    private ITweetRepository TweetRepository { get; set; }

    public TweetsController(
        ILogger<TweetsController> logger,
        ITweetRepository tweetRepository
    )
    {
        Logger = logger;
        TweetRepository = tweetRepository;
    }

    [HttpGet("")]
    public async Task<IEnumerable<Tweet>> GetAll()
    {
        //Could be a lot of data, probably dangerous if not paged :)
        return await TweetRepository.GetAll();
    }

    [HttpGet("TopHashtags")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<HashtagCount>>> GetHashtagCount(int count = 10)
    {
        if (count <= 0)
            return BadRequest("Specified count should be more than 0");

        return Ok(await TweetRepository.GetTopHashtags(count));
    }

    [HttpGet("Count")]
    public async Task<long> GetCount()
    {
        return await TweetRepository.GetCount();
    }
}
