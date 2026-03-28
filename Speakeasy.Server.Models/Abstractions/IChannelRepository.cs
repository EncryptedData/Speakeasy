using Speakeasy.Server.Models.Database;

namespace Speakeasy.Server.Models.Abstractions;

public interface IChannelRepository : IRepository<Channel>
{
    Task<IEnumerable<Channel>> GetChannelsForGroup(Guid groupId, bool trackEntities = false);
}