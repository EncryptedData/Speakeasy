using Speakeasy.Server.Common.Exceptions;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Exceptions;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Models.Converters;

public class GroupRoleClaimModelConverter : IModelConverter<GroupRoleClaim, GroupRoleClaimDto>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    
    public GroupRoleClaimModelConverter(ICurrentUserProvider currentUserProvider)
    {
        _currentUserProvider = currentUserProvider;
    }
    
    public async Task<GroupRoleClaim> ToDatabaseModelAsync(IUnitOfWork uow, GroupRoleClaimDto dto)
    {
        var group = await uow.GroupRepository.GetByIdAsync(dto.GroupId);
        ExceptionUtil.ThrowIfNull(group, () => new GroupNotFoundException(dto.GroupId));
        
        GroupRole? groupRole = group.Roles.Find(gr => gr.Name.Equals(dto.RoleName));
        ExceptionUtil.ThrowIfNull(groupRole, () => new RoleNotFoundException(dto.RoleName));
        
        return new GroupRoleClaim
        {
            Id = Guid.NewGuid(),
            User = await _currentUserProvider.GetCurrentUserAsync(),
            GroupRole = groupRole
        };
    }

    public GroupRoleClaimDto ToTransmissionModel(GroupRoleClaim entity)
    {
        return new GroupRoleClaimDto
        {
            Id = entity.Id,
            GroupId = entity.GroupRole.Id,
            RoleName = entity.GroupRole.Name,
        };
    }

    public void UpdateDatabaseModelAsync(GroupRoleClaim entity, GroupRoleClaimDto dto)
    {
        // Don't update, only remove or insert.
        throw new NotSupportedException();
    }
}