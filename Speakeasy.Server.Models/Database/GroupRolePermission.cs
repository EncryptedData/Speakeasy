using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Enums;

namespace Speakeasy.Server.Models.Database;

[PrimaryKey(nameof(Id))]
public class GroupRolePermission : IEntity
{
    public Guid Id { get; set; }
    
    public GroupRole GroupRole { get; set; }
    
    public PermissionType Type { get; set; }
}