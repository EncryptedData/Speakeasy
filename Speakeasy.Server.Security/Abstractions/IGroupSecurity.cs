using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Database.Repositories;
using Speakeasy.Server.Models.Enums;

namespace Speakeasy.Server.Security.Abstractions;

public interface IGroupSecurity
{ 
        public Task<bool> HasPermissionInGroup(Group group, PermissionType permissionType);
}