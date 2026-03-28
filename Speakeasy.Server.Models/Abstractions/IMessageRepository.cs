using Speakeasy.Server.Models.Database;

namespace Speakeasy.Server.Models.Abstractions;

public interface IMessageRepository : IRepository<Message>
{
    public Task<IEnumerable<Message>> GetMessagesForChannelAsync(
        Guid channelId, 
        Guid? lastMessageId, 
        int take = 10,
        bool trackEntities = false);
}