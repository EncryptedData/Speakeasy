using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class GroupRoleRepository : BaseRepository<GroupRole>, IGroupRoleRepository
{
    public GroupRoleRepository(SpeakeasyDbContext context) : 
        base(context.GroupRoles)
    {
        
    }

    protected override IQueryable<GroupRole> ApplyIncludes(IQueryable<GroupRole> query)
    {
        return query.Include(g => g.Group)
            .ThenInclude(g => g.Roles);
    }

    public IAsyncEnumerable<GroupRole> GetAllByGroupIdAsyncEnumerable(Guid groupId, bool trackEntities = false)
    {
        IQueryable<GroupRole> query = _db;

        if (trackEntities is false)
        {
            query = query.AsNoTracking();
        }
        
        query = ApplyIncludes(query);

        return query.AsAsyncEnumerable();
    }
}