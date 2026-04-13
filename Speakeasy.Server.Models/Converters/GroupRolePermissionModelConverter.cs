using Speakeasy.Server.Common.Exceptions;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Exceptions;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Models.Converters;

public class GroupRolePermissionModelConverter : IModelConverter<GroupRolePermission, GroupRolePermissionDto>
{
    public async Task<GroupRolePermission> ToDatabaseModelAsync(IUnitOfWork uow, GroupRolePermissionDto dto)
    {
        var role = await uow.GroupRoleRepository.GetByIdAsync(dto.GroupRoleId);
        ExceptionUtil.ThrowIfNull<Exception>(role, () => new RoleNotFoundException(dto.GroupRoleId));
        
        var permission = new GroupRolePermission
        {
            Id = Guid.NewGuid(),
            Type = dto.PermissionType,
            GroupRole = role
        };
        
        role.GroupRolePermissions.Add(permission);
        
        return permission;
    }

    public GroupRolePermissionDto ToTransmissionModel(GroupRolePermission entity)
    {
        return new GroupRolePermissionDto
        {
            Id = entity.Id,
            PermissionType = entity.Type,
            GroupRoleId = entity.GroupRole.Id
        };
    }

    public void UpdateDatabaseModelAsync(GroupRolePermission entity, GroupRolePermissionDto dto)
    {
        // We never want to update a permission, just remove or add it.
        throw new NotSupportedException();
    }
}