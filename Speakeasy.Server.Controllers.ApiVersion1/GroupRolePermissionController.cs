using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public class GroupRolePermissionController : BaseRepositoryController<GroupRolePermission, GroupRolePermissionDto>
{
    public GroupRolePermissionController(
        IModelConverter<GroupRolePermission, GroupRolePermissionDto> converter, 
        IUnitOfWork uow) : 
        base(uow.GroupRolePermissionRepository, converter, uow)
    {
    }

    public override Task<ActionResult> PutAsync(GroupRolePermissionDto dto)
    {
        return base.PutAsync(dto);
    }

    public override async Task<ActionResult<GroupRolePermissionDto>> PostAsync(GroupRolePermissionDto dto)
    {
        if (await _unitOfWork.GroupRoleRepository.GetByIdAsync(dto.GroupRoleId) is not { } role)
        {
            return NotFound();
        }
        
        return await base.PostAsync(dto);
    }

    protected override Task OnEntityCreatedAsync(GroupRolePermissionDto dto)
    {
        return Task.CompletedTask;
    }

    protected override Task OnEntityUpdatedAsync(Guid id)
    {
        return Task.CompletedTask;
    }

    protected override Task OnEntityDeletedAsync(Guid id)
    {
        return Task.CompletedTask;
    }
}