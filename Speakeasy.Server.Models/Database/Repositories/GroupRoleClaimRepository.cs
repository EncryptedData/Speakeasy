using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class GroupRoleClaimRepository : BaseRepository<GroupRoleClaim>, IGroupRoleClaimRepository
{
    public GroupRoleClaimRepository(SpeakeasyDbContext context) : base(context.GroupRoleClaims)
    {
    }

    protected override IQueryable<GroupRoleClaim> ApplyIncludes(IQueryable<GroupRoleClaim> query)
    {
        return query
            .Include(grc => grc.GroupRole);
    }

    public IAsyncEnumerable<GroupRoleClaim> GetGroupRoleClaimsByGroupRoleAsyncEnumerable(Guid groupRoleId, bool trackEntities = true)
    {
        IQueryable<GroupRoleClaim> query = _db;

        if (!trackEntities)
        {
            query = query.AsNoTracking();
        }
        
        query = ApplyIncludes(query);

        return query
            .Where(gr => gr.GroupRole.Id == groupRoleId).AsAsyncEnumerable();
    }

    public IAsyncEnumerable<GroupRoleClaim> GetGroupRoleClaimsByGroupAsyncEnumerable(Guid groupId,
        bool trackEntities = true)
    {
        IQueryable<GroupRoleClaim> query = _db;

        if (!trackEntities)
        {
            query = query.AsNoTracking();
        }
        
        query = ApplyIncludes(query);

        return query
            .Where(gr => gr.GroupRole.Group.Id == groupId).AsAsyncEnumerable();
    }

    public IAsyncEnumerable<GroupRoleClaim> GetClaimByUserAndGroupAsyncEnumerable(string userId, Guid groupId, bool trackEntities = true)
    {
        IQueryable<GroupRoleClaim> query = _db;

        if (!trackEntities)
        {
            query = query.AsNoTracking();
        }
        
        query = ApplyIncludes(query);

        return query
            .Where(gr => gr.GroupRole.Group.Id == groupId && gr.User.Id == userId).AsAsyncEnumerable();
    }
}