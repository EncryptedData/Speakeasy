namespace Speakeasy.Server.Models.Abstractions;

public interface IRepository<T>
    where T : IEntity
{
    Task<T?> GetByIdAsync(Guid id, bool trackEntity = true);

    Task<bool> ContainsAsync(Guid id);

    Task AddAsync(T entity);

    Task AddRangeAsync(IEnumerable<T> entities);

    void Remove(T entity);
}