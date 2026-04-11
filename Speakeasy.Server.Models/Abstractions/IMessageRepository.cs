using Speakeasy.Server.Models.Database;

namespace Speakeasy.Server.Models.Abstractions;

public interface IMessageRepository : IRepository<Message>
{
    Task<IEnumerable<Message>> GetMessagesForChannelAsync(
        Guid channelId, 
        Guid? lastMessageId, 
        int take = 10,
        bool trackEntities = false);

    Task AddMessageReactionAsync(MessageReaction reaction);

    void RemoveMessageReaction(MessageReaction reaction);
}