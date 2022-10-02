namespace TwitterApiExample.Repositories;

public interface IRepository<T>
{
    Task Save(T item);
    Task<IList<T>> GetAll();
    Task<int> GetCount();
}

