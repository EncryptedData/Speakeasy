using System.ComponentModel.DataAnnotations;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Transmission;

public class ChannelDto : ITransmissionNamedEntity
{
    public Guid? Id { get; set; }
    
    [Required]
    [Length(1, ModelConstants.Limits.MaxNameLength)]
    public string Name { get; set; }
    
    [Required]
    public Guid GroupId { get; set; }
    
    public string? CreatedBy { get; set; }
}