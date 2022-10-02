using TwitterApiExample.Models;

namespace TwitterApiExample.Repositories;
public interface ITweetRepository: IRepository<Tweet>, IAsyncDisposable
{
    Task InitDb(); //Not amazing, but normally this wouldn't be a consideration with a normal DB. 
    Task ClearDb(); //Not amazing, but normally this wouldn't be a consideration with a normal DB. 
    Task<IList<HashtagCount>> GetTopHashtags(int top);
}

