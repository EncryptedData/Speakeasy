using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class CustomEmojiRepository : BaseRepository<CustomEmoji>, ICustomEmojiRepository
{
    public CustomEmojiRepository(SpeakeasyDbContext context) : base(context.CustomEmojis)
    {
    }

    protected override IQueryable<CustomEmoji> ApplyIncludes(IQueryable<CustomEmoji> query)
    {
        return query.Include(e => e.Group)
            .Include(e => e.Image)
            .Include(e => e.Author);
    }

    public IAsyncEnumerable<CustomEmoji> GetAllForGroupAsyncEnumerable(Guid groupId, bool includeGlobal = true,
        bool trackEntities = false)
    {
        IQueryable<CustomEmoji> query = _db;

        if (trackEntities is false)
        {
            query = query.AsNoTracking();
        }

        query = ApplyIncludes(query);

        query = includeGlobal
            ? query.Where(e => e.Group == null || e.Group.Id == groupId)
            : query.Where(e => e.Group.Id == groupId); 
        
        return query
            .AsAsyncEnumerable();
    }

    public async Task<bool> ContainsNameAsync(string name)
    {
        return await _db.AnyAsync(e => e.Name == name);
    }
}