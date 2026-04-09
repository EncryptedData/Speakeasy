using System.ComponentModel.DataAnnotations;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Transmission;

public class GroupRoleDto : ITransmissionNamedEntity
{
    public Guid? Id { get; set; }
    
    [Required]
    [Length(1, ModelConstants.Limits.MaxNameLength)]
    public string Name { get; set; }
    
    [Required]
    public Guid GroupId { get; set; }
    
    [Required]
    public int Hierarchy { get; set; }
    
    public bool? IsDefault { get; set; }
}