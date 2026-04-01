using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbSet<User> _db;

    public UserRepository(SpeakeasyDbContext context)
    {
        _db = context.Users;
    }

    IQueryable<User> ApplyIncludes(IQueryable<User> query)
    {
        return query.Include(e => e.ProfilePicture);
    }
    
    public async Task<User?> GetUserByIdAsync(string id, bool trackEntities = true)
    {
        IQueryable<User> query = _db;
       
        if(trackEntities is false)
        {
            query = query.AsNoTracking();
        }

        query = ApplyIncludes(query);

        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> ContainsIdAsync(string id)
    {
        return await _db.AnyAsync(e => e.Id == id);
    }

    public async Task AddAsync(User user)
    {
        await _db.AddAsync(user);
    }

    public void Remove(User user)
    {
        _db.Remove(user);
    }
}