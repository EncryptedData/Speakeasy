using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class ChannelRepository : BaseRepository<Channel>, IChannelRepository
{
    public ChannelRepository(SpeakeasyDbContext context) : 
        base(context.Channels)
    {
    }
}