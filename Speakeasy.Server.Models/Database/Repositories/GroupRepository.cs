using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class GroupRepository : BaseRepository<Group>, IGroupRepository
{
    public GroupRepository(SpeakeasyDbContext context) : 
        base(context.Groups)
    {
    }

    protected override IQueryable<Group> ApplyIncludes(IQueryable<Group> query)
    {
        return query
            .Include(e => e.Channels)
            .Include(e => e.Roles);
    }

    public IAsyncEnumerable<Group> GetAll(bool trackEntities = false)
    {
        IQueryable <Group> query = _db;

        if (trackEntities is false)
        {
            query = query.AsNoTracking();
        }

        query = ApplyIncludes(query);

        return query.AsAsyncEnumerable();
    }
}