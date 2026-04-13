using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Models.Converters;

public class GroupRoleModelConverter : IModelConverter<GroupRole, GroupRoleDto>
{
    public async Task<GroupRole> ToDatabaseModelAsync(IUnitOfWork uow, GroupRoleDto dto)
    {
        var group = await uow.GroupRepository.GetByIdAsync(dto.GroupId);
        ArgumentNullException.ThrowIfNull(group);
        
        return new GroupRole
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            // We ensure that the role hierarchy is continuous between 0 -> group.Roles.Count - 1
            Hierarchy = group.Roles.Count,
            IsDefault = dto.IsDefault ?? false,
            Group = group,
            GroupRolePermissions = []
        };
    }

    public GroupRoleDto ToTransmissionModel(GroupRole entity)
    {
        return new GroupRoleDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Hierarchy = entity.Hierarchy,
            IsDefault = entity.IsDefault,
            GroupId = entity.Group.Id
        };
    }

    public void UpdateDatabaseModelAsync(GroupRole entity, GroupRoleDto dto)
    {
        entity.Name = dto.Name;
        entity.Hierarchy = dto.Hierarchy;

        if (dto.IsDefault is not null)
        {
            entity.IsDefault = dto.IsDefault is true;
        }
    }
}