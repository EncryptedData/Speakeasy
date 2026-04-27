using Speakeasy.Server.Models.Database;

namespace Speakeasy.Server.Models.Abstractions;

public interface IGroupRepository : IRepository<Group>
{
    IAsyncEnumerable<Group> GetAll(bool trackEntities = false);

    Task AddClaimAsync(GroupUserClaim claim);
    
    void RemoveClaim(GroupUserClaim claim);
}