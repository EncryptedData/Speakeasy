using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Enums;
using Speakeasy.Server.Security.Abstractions;

namespace Speakeasy.Server.Security;

public class GroupSecurity : IGroupSecurity
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GroupSecurity(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }
    
    public async Task<bool> HasPermissionInGroup(
        Group group,
        PermissionType permissionType
        )
    {
        var user = await _currentUserProvider.GetCurrentUserAsync();
        var claimsInGroup =
            await _unitOfWork.GroupRoleClaimRepository.GetClaimByUserAndGroupAsyncEnumerable(user.Id, group.Id).ToListAsync();

        return claimsInGroup.Any(claim => 
            claim.GroupRole.GroupRolePermissions.Any(gr => gr.Type == permissionType));
    }
}