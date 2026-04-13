using Speakeasy.Server.Models.Database;

namespace Speakeasy.Server.Models.Abstractions;

public interface IGroupRolePermissionRepository : IRepository<GroupRolePermission>
{
    IAsyncEnumerable<GroupRolePermission> GetAllPermissionsByGroupIdAsyncEnumerable(Guid groupId,
        bool trackEntities = false);
}