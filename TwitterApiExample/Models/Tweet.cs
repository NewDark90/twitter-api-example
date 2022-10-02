namespace TwitterApiExample.Models;

public class Tweet
{
    public string? Id { get; set; }
    public string? Text { get; set; }
    public string? AuthorId { get; set; }
    public ICollection<string> Hashtags { get; set; } = new List<string>();
}

