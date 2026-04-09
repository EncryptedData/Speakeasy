using Speakeasy.Server.Models.Database;

namespace Speakeasy.Server.Models.Abstractions;

public interface IGroupRoleRepository : IRepository<GroupRole>
{
    IAsyncEnumerable<GroupRole> GetAllByGroupIdAsyncEnumerable(Guid groupId, bool trackEntities = false);
}