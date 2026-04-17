using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Models.Converters;

public class GroupModelConverter : IModelConverter<Group, GroupDto>
{
    private readonly ICurrentUserProvider _currentUserProvider;

    public GroupModelConverter(ICurrentUserProvider currentUserProvider)
    {
        _currentUserProvider = currentUserProvider;
    }
        
    public async Task<Group> ToDatabaseModelAsync(IUnitOfWork uow, GroupDto dto)
    {
        var group = new Group
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Channels = [],
            Roles = [],
            Claims = [],
            CreatedOn = DateTime.UtcNow
        };
        
        // Generate default group.
        var groupRole = new GroupRole
        {
            Id = Guid.NewGuid(),
            Name = "Default",
            Group = group,
            Hierarchy = 0,
            IsDefault = true
        };
        
        var groupUserClaim = new GroupUserClaim
        {
            Id = Guid.NewGuid(),
            Group = group,
            User = await _currentUserProvider.GetCurrentUserAsync(),
            Permission = GroupUserClaimPermission.Admin
        };
        
        group.Claims.Add(groupUserClaim);
        
        group.Roles.Add(groupRole);
        
        await uow.GroupRoleRepository.AddAsync(groupRole);
        await uow.GroupRepository.AddClaimAsync(groupUserClaim);
        
        return group;
    }

    public GroupDto ToTransmissionModel(Group entity)
    {
        return new GroupDto()
        {
            Id = entity.Id,
            Name = entity.Name,
            Channels = entity.Channels.Select(e => e.Id).ToList(),
            CreatedOn = entity.CreatedOn,
        };
    }

    public void UpdateDatabaseModelAsync(Group entity, GroupDto dto)
    {
        entity.Name = dto.Name;
    }
}