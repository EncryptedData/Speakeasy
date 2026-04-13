using System.ComponentModel.DataAnnotations;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Transmission;

public class GroupRoleClaimDto : ITransmissionEntity
{
    public Guid? Id { get; set; }
    
    [Required]
    public Guid GroupId { get; set; }
    
    [Required] 
    public string RoleName { get; set; }
}