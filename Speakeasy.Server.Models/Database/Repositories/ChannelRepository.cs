using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class ChannelRepository : BaseRepository<Channel>, IChannelRepository
{
    public ChannelRepository(SpeakeasyDbContext context) : 
        base(context.Channels)
    {
    }

    protected override IQueryable<Channel> ApplyIncludes(IQueryable<Channel> query)
    {
        return query
            .Include(e => e.CreatedBy)
            .Include(e => e.Group);
    }

    public async Task<IEnumerable<Channel>> GetChannelsForGroup(Guid groupId, bool trackEntities)
    {
        IQueryable<Channel> query = _db;

        if (trackEntities is false)
        {
            query = query.AsNoTracking();
        }

        query = ApplyIncludes(query);

        var list = await query.Where(e => e.Group.Id == groupId).ToListAsync();

        return list;
    }
}