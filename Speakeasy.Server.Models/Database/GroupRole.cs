using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database;

#pragma warning disable CS8618

[PrimaryKey(nameof(Id))]
public class GroupRole : INamedEntity
{
    public Guid Id { get; set; }
    
    public Group Group { get; set; }
    
    public string Name { get; set; }
    
    public bool IsDefault { get; set; }
    
    public int Hierarchy { get; set; }
    
    public List<GroupRolePermission> GroupRolePermissions { get; set; }
}