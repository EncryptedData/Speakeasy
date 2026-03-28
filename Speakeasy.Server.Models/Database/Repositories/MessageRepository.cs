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
        return query.Include(e => e.Author);
    }

    public async Task<IEnumerable<Message>> GetMessagesForChannelAsync(
        Guid channelId, 
        Guid? lastMessageId,
        int take = 10, 
        bool trackEntities = false)
    {
        DateTime? lastMessageTime = null;
        if (lastMessageId is not null)
        {
            var message = await GetByIdAsync(lastMessageId.Value, false);
            if (message is not null)
            {
                lastMessageTime = message.CreatedOn;
            }
        }
        
        IQueryable<Message> query = _db.AsQueryable();

        if (trackEntities is false)
        {
            query = query.AsNoTracking();
        }

        query = ApplyIncludes(query);

        if (lastMessageId is not null && lastMessageTime is not null)
        {
            query = query
                .Where(e => e.CreatedOn <= lastMessageTime)
                .Where(e => e.Id != lastMessageId);
        }

        var result = await query
            .Where(e => e.Channel.Id == channelId)
            .OrderByDescending(e => e.CreatedOn)
            .Take(take)
            .ToListAsync();

        return result;
    }
}