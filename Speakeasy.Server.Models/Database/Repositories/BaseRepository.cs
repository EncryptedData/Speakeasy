using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public abstract class BaseRepository<T> : IRepository<T> 
    where T : class, IEntity
{
    protected DbSet<T> _db;
    
    protected BaseRepository(DbSet<T> db)
    {
        _db = db;
    }
    
    public async Task<T?> GetByIdAsync(Guid id, bool trackEntity = true)
    {
        IQueryable<T> query = _db;

        if (trackEntity is false)
        {
            query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> ContainsAsync(Guid id)
    {
        return await _db.AsNoTracking().AnyAsync(e => e.Id == id);
    }

    public async Task AddAsync(T entity)
    {
        await _db.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _db.AddRangeAsync(entities);
    }

    public void Remove(T entity)
    {
        _db.Remove(entity);
    }
}