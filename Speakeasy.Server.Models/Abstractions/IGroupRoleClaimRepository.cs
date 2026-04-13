using Speakeasy.Server.Models.Database;

namespace Speakeasy.Server.Models.Abstractions;

public interface IGroupRoleClaimRepository : IRepository<GroupRoleClaim>
{
    IAsyncEnumerable<GroupRoleClaim> GetGroupRoleClaimsByGroupRoleAsyncEnumerable(Guid groupRoleId, bool trackEntities = true);

    IAsyncEnumerable<GroupRoleClaim> GetGroupRoleClaimsByGroupAsyncEnumerable(Guid groupId,
        bool trackEntities = true);
    
    IAsyncEnumerable<GroupRoleClaim> GetClaimByUserAndGroupAsyncEnumerable(string userId, Guid groupId,
        bool trackEntities = true);
}