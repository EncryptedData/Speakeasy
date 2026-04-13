using System.ComponentModel.DataAnnotations;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Enums;

namespace Speakeasy.Server.Models.Transmission;

public class GroupRolePermissionDto : ITransmissionEntity
{
    public Guid? Id { get; set; }
    
    [Required]
    public Guid GroupRoleId { get; set; }
    
    public PermissionType PermissionType { get; set; }
}