using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class GroupRepository : BaseRepository<Group>, IGroupRepository
{
    private readonly DbSet<GroupUserClaim> _claimsDb;
    
    public GroupRepository(SpeakeasyDbContext context) : 
        base(context.Groups)
    {
        _claimsDb = context.GroupUserClaims;
    }

    protected override IQueryable<Group> ApplyIncludes(IQueryable<Group> query)
    {
        return query
            .Include(e => e.Channels)
            .Include(e => e.Roles)
            .Include(e => e.Claims)
            .ThenInclude(e => e.User);
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

    public async Task AddClaimAsync(GroupUserClaim claim)
    {
       await _claimsDb.AddAsync(claim);
    }

    public void RemoveClaim(GroupUserClaim claim)
    {
        _claimsDb.Remove(claim);
    }
}