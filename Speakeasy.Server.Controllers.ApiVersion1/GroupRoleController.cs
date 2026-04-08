using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Controllers.ApiVersion1.Hubs;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Converters;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public class GroupRoleController : BaseRepositoryController<GroupRole, GroupRoleDto>
{
    
    public GroupRoleController(
        IModelConverter<GroupRole, GroupRoleDto> converter, 
        IUnitOfWork uow) : 
        base(uow.GroupRoleRepository, converter, uow)
    {
        
    }

    public override async Task<ActionResult> PutAsync(GroupRoleDto dto)
    {
        if(dto.Id is null) return NotFound();
        
        // Get the group
        var groupSpecified = await _unitOfWork.GroupRepository.GetByIdAsync(dto.GroupId);
        
        // Then check if the group role is found. If not, ban hacker, or return NotFound, whichever is easier.
        if (groupSpecified?.Roles.Find(g => g.Id == dto.Id) is not { } r )
        {
            return NotFound();
        }
        
        if (dto.IsDefault is not null)
        {
            // Make sure at least one other role is set as 'default' if the user is trying to set a role as not default.
            // (per discord discussion - this will probably be refactored later)
            if (dto.IsDefault is false
                && r.IsDefault
                && !groupSpecified.Roles.Any(g => g.IsDefault))
            {
                return BadRequest("Default cannot be set to false on this role when no other default roles remain!");
            }

            // Else, just set it to true or false.
            r.IsDefault = dto.IsDefault is true;
        }

        // Otherwise change the hierarchy,
        if (groupSpecified.Roles.Any(g => g.Hierarchy == dto.Hierarchy && g.Id != dto.Id))
        {
            foreach (var gr in groupSpecified.Roles)
            {
                // If it's the object we found above, it's the object we're changing. Simply change the hierarchy value.
                if (gr.Id == r.Id)
                {
                    gr.Hierarchy = dto.Hierarchy;
                }
                else
                // Otherwise, the hierarchy needs to incremented for each item in the list equal to or greater than the item being set.
                if (gr.Hierarchy >= dto.Hierarchy)
                {
                    ++gr.Hierarchy;
                }
            }
        }
        
        return await base.PutAsync(dto);
    }

    public override async Task<ActionResult<GroupRoleDto>> PostAsync(GroupRoleDto dto)
    {
        var group = await _unitOfWork.GroupRepository.GetByIdAsync(dto.GroupId);

        if (group is null)
        {
            return NotFound();
        }

        if (group.Roles.Any(
                r => r.Name.Equals(dto.Name, StringComparison.CurrentCultureIgnoreCase)))
        {
            return BadRequest("A role already contains that name!");
        }
        
        return await base.PostAsync(dto);
    }

    public override async Task<ActionResult> DeleteAsync(Guid id)
    {
        var selectedRole = _unitOfWork.GroupRoleRepository.GetByIdAsync(id).Result;

        if (selectedRole is null)
        {
            return NotFound();
        }

        var roles = selectedRole.Group.Roles;

        if (roles.Count == 1)
        {
            return BadRequest("You can't delete the last role remaining.");
        }
        
        roles.Remove(selectedRole);

        if (!roles.Any(gr => gr.IsDefault))
        {
            return BadRequest("You can't delete the last default role.");
        }
        
        // If each role is greater than the one being deleted, then simply subtract one from it.
        foreach (var r in roles)
        {
            if (r.Hierarchy > selectedRole.Hierarchy)
            {
                --r.Hierarchy;
            }
        }
        
        // When permissions & RBAC gets added, we need to remove all users in the role being deleted.
        
        return await base.DeleteAsync(id);
    }

    protected override Task OnEntityCreatedAsync(GroupRoleDto dto)
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