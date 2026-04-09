using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class MessageRepository : BaseRepository<Message>, IMessageRepository
{
    private DbSet<MessageReaction> _reactionsDb;
    
    public MessageRepository(SpeakeasyDbContext context) : 
        base(context.Messages)
    {
        _reactionsDb = context.Set<MessageReaction>();
    }

    protected override IQueryable<Message> ApplyIncludes(IQueryable<Message> query)
    {
        return query.Include(e => e.Author)
            .Include(e => e.Channel)
            .Include(e => e.Reactions)
                .ThenInclude(e => e.CustomEmoji)
            .Include(e => e.Reactions)
                .ThenInclude(e => e.Reactors);
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

    public async Task AddMessageReactionAsync(MessageReaction reaction)
    {
        await _reactionsDb.AddAsync(reaction);
    }

    public void RemoveMessageReaction(MessageReaction reaction)
    {
        _reactionsDb.Remove(reaction);
    }
}