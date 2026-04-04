using Speakeasy.Server.Models.Database;

namespace Speakeasy.Server.Models.Abstractions;

public interface ICustomEmojiRepository : IRepository<CustomEmoji>
{

    IAsyncEnumerable<CustomEmoji> GetAllForGroupAsyncEnumerable(Guid groupId, bool includeGlobal = true,
        bool trackEntities = false);
}