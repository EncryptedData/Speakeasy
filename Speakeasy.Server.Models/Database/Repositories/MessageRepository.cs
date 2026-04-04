using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class MessageRepository : BaseRepository<Message>, IMessageRepository
{
    public MessageRepository(SpeakeasyDbContext context) : 
        base(context.Messages)
    {
    }

    protected override IQueryable<Message> ApplyIncludes(IQueryable<Message> query)
    {
        return query.Include(e => e.Author)
            .Include(e => e.Channel);
    }

    public async Task<IEnumerable<Message>> GetMessagesForChannelAsync(
        Guid channelId, 
        Guid? lastMessageId,
        int take = 10, 
        bool trackEntities = false)
    {
        IQueryable<Message> query = _db.AsQueryable();

        if (trackEntities is false)
        {
            query = query.AsNoTracking();
        }

        query = ApplyIncludes(query);

        if (lastMessageId is not null)
        {
            query = query
                .Where(e => e.Id.CompareTo(lastMessageId.Value) < 0);
        }

        var result = await query
            .Where(e => e.Channel.Id == channelId)
            .OrderByDescending(e => e.Id)
            .Take(take)
            .ToListAsync();

        return result;
    }
}