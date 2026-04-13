using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class GroupRolePermissionRepository : BaseRepository<GroupRolePermission>, IGroupRolePermissionRepository
{
    public GroupRolePermissionRepository(SpeakeasyDbContext db) : base(db.GroupRolePermissions)
    {
    }

    protected override IQueryable<GroupRolePermission> ApplyIncludes(IQueryable<GroupRolePermission> query)
    {
        return query.Include(gr => gr.GroupRole);
    }


    public IAsyncEnumerable<GroupRolePermission> GetAllPermissionsByGroupIdAsyncEnumerable(Guid groupId, bool trackEntities = false)
    {
        IQueryable<GroupRolePermission> query = _db;

        if (trackEntities is false)
        {
            query = query.AsNoTracking();
        }
        
        query = ApplyIncludes(query);

        return query.Where(grp => grp.GroupRole.Group.Id == groupId).AsAsyncEnumerable();
    }
}