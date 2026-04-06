using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class CustomEmojiRepository : BaseRepository<CustomEmoji>, ICustomEmojiRepository
{
    public CustomEmojiRepository(SpeakeasyDbContext context) : base(context.CustomEmojis)
    {
    }

    public IAsyncEnumerable<CustomEmoji> GetAllForGroupAsyncEnumerable(Guid groupId, bool includeGlobal = true,
        bool trackEntities = false)
    {
        IQueryable<CustomEmoji> query = _db;

        if (trackEntities is false)
        {
            query = query.AsNoTracking();
        }

        query = includeGlobal
            ? query.Where(e => e.Group == null || e.Group.Id == groupId)
            : query.Where(e => e.Group.Id == groupId); 
        
        return query.AsAsyncEnumerable();
    }

    public async Task<bool> ContainsNameAsync(string name)
    {
        return await _db.AnyAsync(e => e.Name == name);
    }
}