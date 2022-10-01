using TwitterApiExample.Models;

namespace TwitterApiExample.Repositories;
public interface ITweetRespository: IRepository<Tweet>
{
    Task<IList<string>> GetTopHashtags();
}

