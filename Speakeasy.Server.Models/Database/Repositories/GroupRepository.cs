using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class GroupRepository : BaseRepository<Group>, IGroupRepository
{
    public GroupRepository(SpeakeasyDbContext context) : 
        base(context.Groups)
    {
    }
}