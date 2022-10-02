using TwitterApiExample.Models;

namespace TwitterApiExample.Repositories;
public interface ITweetRepository: IRepository<Tweet>
{
    Task InitDb(); //Not amazing, but normally this wouldn't be a consideration with a normal DB. 
    Task<IList<string>> GetTopHashtags(int top);
}

